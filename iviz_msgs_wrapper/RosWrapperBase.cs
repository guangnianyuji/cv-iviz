using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Iviz.Msgs;
using Iviz.Msgs.GeometryMsgs;
using Iviz.Msgs.IvizMsgs;
using Iviz.Msgs.RosgraphMsgs;
using Iviz.Msgs.StdMsgs;
using ISerializable = Iviz.Msgs.ISerializable;

namespace Iviz.MsgsWrapper
{
    using static RosWrapperBase;

    /// <summary>
    /// Determines the name of the ROS message. Must be a string constant.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MessageNameAttribute : Attribute
    {
    }

    /// <summary>
    /// Tells the wrapper that the array with this attribute has a fixed length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FixedSizeArrayAttribute : Attribute
    {
        public uint Size { get; }
        public FixedSizeArrayAttribute(uint size) => Size = size;
    }

    /// <summary>
    /// Tells the wrapper to rename the property to the given name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RenameToAttribute : Attribute
    {
        public string Name { get; }
        public RenameToAttribute(string name) => Name = name;
    }

    public class RosIncompleteWrapperException : RosInvalidMessageException
    {
        public RosIncompleteWrapperException(string msg) : base(msg)
        {
        }
    }

    internal static class RosWrapperBase
    {
        public static readonly Dictionary<Type, string> BuiltInNames = new()
        {
            [typeof(bool)] = "bool",
            [typeof(char)] = "uint16", // char doesn't translate to uint8, it's 16 bit!
            [typeof(sbyte)] = "char",
            [typeof(byte)] = "byte",
            [typeof(short)] = "int16",
            [typeof(ushort)] = "uint16",
            [typeof(int)] = "int32",
            [typeof(uint)] = "uint32",
            [typeof(long)] = "int64",
            [typeof(ulong)] = "uint64",
            [typeof(float)] = "float32",
            [typeof(double)] = "float64",
            [typeof(time)] = "time",
            [typeof(duration)] = "duration",
            [typeof(string)] = "string",
        };

        public static string CompressDependencies(string rosDependencies)
        {
            byte[] inputBytes = BuiltIns.UTF8.GetBytes(rosDependencies);
            using var outputStream = new MemoryStream();
            using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gZipStream.Write(inputBytes, 0, inputBytes.Length);
            }

            return Convert.ToBase64String(outputStream.ToArray());
        }

        public static string CreateMd5(string inputForMd5)
        {
#pragma warning disable CA5351
            using MD5 md5Hash = MD5.Create();
#pragma warning restore CA5351
            byte[] data = md5Hash.ComputeHash(BuiltIns.UTF8.GetBytes(inputForMd5));
            var md5HashBuilder = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                md5HashBuilder.Append(b.ToString("x2", BuiltIns.Culture));
            }

            return md5HashBuilder.ToString();
        }

        public static void AddDependency(StringBuilder rosDependencies, Type type, HashSet<Type> alreadyWritten,
            bool first = false)
        {
            if (alreadyWritten.Contains(type))
            {
                return;
            }

            const string dependencySeparator =
                "================================================================================";

            alreadyWritten.Add(type);
            if (!first)
            {
                rosDependencies.AppendLine(dependencySeparator);
                rosDependencies.Append("MSG: ").AppendLine(BuiltIns.GetMessageType(type));

                string typeDependencies = BuiltIns.DecompressDependencies(type);
                int separatorIndex = typeDependencies.IndexOf(dependencySeparator, StringComparison.Ordinal);
                if (separatorIndex == -1)
                {
                    rosDependencies.Append(typeDependencies);
                }
                else
                {
                    rosDependencies.Append(typeDependencies, 0, separatorIndex);
                }
            }

            if (type.IsValueType)
            {
                foreach (var field in type.GetFields())
                {
                    if (typeof(IMessage).IsAssignableFrom(field.FieldType))
                    {
                        AddDependency(rosDependencies, field.FieldType, alreadyWritten);
                    }
                    else if (field.FieldType.IsArray &&
                             typeof(IMessage).IsAssignableFrom(field.FieldType.GetElementType()))
                    {
                        AddDependency(rosDependencies, field.FieldType.GetElementType()!, alreadyWritten);
                    }
                }

                return;
            }

            foreach (var property in type.GetProperties())
            {
                if (property.GetGetMethod() == null
                    || property.GetSetMethod() == null
                    || property.GetCustomAttribute<IgnoreDataMemberAttribute>() != null
                )
                {
                    continue;
                }

                if (typeof(IMessage).IsAssignableFrom(property.PropertyType))
                {
                    AddDependency(rosDependencies, property.PropertyType, alreadyWritten);
                }
                else if (property.PropertyType.IsArray &&
                         typeof(IMessage).IsAssignableFrom(property.PropertyType.GetElementType()))
                {
                    AddDependency(rosDependencies, property.PropertyType.GetElementType()!, alreadyWritten);
                }
            }
        }
    }

    internal sealed partial class RosSerializableDefinition<T> where T : ISerializable, IDeserializable<T>, new()
    {
        sealed class BuilderInfo
        {
            public StringBuilder RosDefinition { get; } = new(200);
            public StringBuilder BufferForMd5 { get; } = new(200);
            public List<IMessageField<T>> Fields { get; } = new();
        }

        static readonly Dictionary<Type, Action<PropertyInfo, BuilderInfo>> Initializers = new()
        {
            [typeof(bool)] = InitializeProperty<bool>,
            [typeof(char)] = InitializeProperty<char>,
            [typeof(sbyte)] = InitializeProperty<sbyte>,
            [typeof(byte)] = InitializeProperty<byte>,
            [typeof(ushort)] = InitializeProperty<ushort>,
            [typeof(short)] = InitializeProperty<short>,
            [typeof(uint)] = InitializeProperty<uint>,
            [typeof(int)] = InitializeProperty<int>,
            [typeof(ulong)] = InitializeProperty<ulong>,
            [typeof(long)] = InitializeProperty<long>,
            [typeof(float)] = InitializeProperty<float>,
            [typeof(double)] = InitializeProperty<double>,
            [typeof(time)] = InitializeProperty<time>,
            [typeof(duration)] = InitializeProperty<duration>,

            [typeof(bool[])] = InitializeArrayProperty<bool>,
            [typeof(char[])] = InitializeArrayProperty<char>,
            [typeof(sbyte[])] = InitializeArrayProperty<sbyte>,
            [typeof(byte[])] = InitializeArrayProperty<byte>,
            [typeof(ushort[])] = InitializeArrayProperty<ushort>,
            [typeof(short[])] = InitializeArrayProperty<short>,
            [typeof(uint[])] = InitializeArrayProperty<uint>,
            [typeof(int[])] = InitializeArrayProperty<int>,
            [typeof(ulong[])] = InitializeArrayProperty<ulong>,
            [typeof(long[])] = InitializeArrayProperty<long>,
            [typeof(float[])] = InitializeArrayProperty<float>,
            [typeof(double[])] = InitializeArrayProperty<double>,
            [typeof(time[])] = InitializeArrayProperty<time>,
            [typeof(duration[])] = InitializeArrayProperty<duration>,

            [typeof(List<bool>)] = InitializeListProperty<bool>,
            [typeof(List<char>)] = InitializeListProperty<char>,
            [typeof(List<sbyte>)] = InitializeListProperty<sbyte>,
            [typeof(List<byte>)] = InitializeListProperty<byte>,
            [typeof(List<ushort>)] = InitializeListProperty<ushort>,
            [typeof(List<short>)] = InitializeListProperty<short>,
            [typeof(List<uint>)] = InitializeListProperty<uint>,
            [typeof(List<int>)] = InitializeListProperty<int>,
            [typeof(List<ulong>)] = InitializeListProperty<ulong>,
            [typeof(List<long>)] = InitializeListProperty<long>,
            [typeof(List<float>)] = InitializeListProperty<float>,
            [typeof(List<double>)] = InitializeListProperty<double>,
            [typeof(List<time>)] = InitializeListProperty<time>,
            [typeof(List<duration>)] = InitializeListProperty<duration>,

            [typeof(string)] = InitializeStringProperty,
            [typeof(string[])] = InitializeStringArrayProperty,
            [typeof(List<string>)] = InitializeStringListProperty,


            // a long list of types to ensure that the AOT stripper does not remove them
            [typeof(Vector3)] = InitializeMessageProperty<Vector3>,
            [typeof(Point)] = InitializeMessageProperty<Point>,
            [typeof(Point32)] = InitializeMessageProperty<Point32>,
            [typeof(Quaternion)] = InitializeMessageProperty<Quaternion>,
            [typeof(Pose)] = InitializeMessageProperty<Pose>,
            [typeof(Transform)] = InitializeMessageProperty<Transform>,
            [typeof(ColorRGBA)] = InitializeMessageProperty<ColorRGBA>,
            [typeof(Color32)] = InitializeMessageProperty<Color32>,
            [typeof(Vector2f)] = InitializeMessageProperty<Vector2f>,
            [typeof(Vector3f)] = InitializeMessageProperty<Vector3f>,
            [typeof(Triangle)] = InitializeMessageProperty<Triangle>,
            [typeof(Header)] = InitializeMessageProperty<Header>,
            [typeof(TransformStamped)] = InitializeMessageProperty<TransformStamped>,
            [typeof(Log)] = InitializeMessageProperty<Log>,

            [typeof(Vector3[])] = InitializeMessageArrayProperty<Vector3>,
            [typeof(Point[])] = InitializeMessageArrayProperty<Point>,
            [typeof(Point32[])] = InitializeMessageArrayProperty<Point32>,
            [typeof(Quaternion[])] = InitializeMessageArrayProperty<Quaternion>,
            [typeof(Pose[])] = InitializeMessageArrayProperty<Pose>,
            [typeof(Transform[])] = InitializeMessageArrayProperty<Transform>,
            [typeof(ColorRGBA[])] = InitializeMessageArrayProperty<ColorRGBA>,
            [typeof(Color32[])] = InitializeMessageArrayProperty<Color32>,
            [typeof(Vector2f[])] = InitializeMessageArrayProperty<Vector2f>,
            [typeof(Vector3f[])] = InitializeMessageArrayProperty<Vector3f>,
            [typeof(Triangle[])] = InitializeMessageArrayProperty<Triangle>,
            [typeof(Header[])] = InitializeMessageArrayProperty<Header>,
            [typeof(TransformStamped[])] = InitializeMessageArrayProperty<TransformStamped>,
            [typeof(Log[])] = InitializeMessageArrayProperty<Log>,
        };

        static string GetPropertyName(MemberInfo property, bool rosIfyName = true)
        {
            string csName = property.GetCustomAttribute<RenameToAttribute>()?.Name
                            ?? property.GetCustomAttribute<DataMemberAttribute>()?.Name
                            ?? property.Name;
            return rosIfyName ? RosIfy(csName) : csName;
        }

        static string RosIfy(string csName)
        {
            var str = new StringBuilder(100);

            bool isFirst = true;
            foreach (char c in csName)
            {
                if (isFirst)
                {
                    str.Append(char.ToLower(c));
                    isFirst = false;
                    continue;
                }

                if (char.IsUpper(c))
                {
                    str.Append('_');
                    str.Append(char.ToLower(c));
                    continue;
                }

                str.Append(c);
            }

            return str.ToString();
        }

        static void InitializeProperty<TField>(PropertyInfo property, BuilderInfo bi) where TField : unmanaged
        {
            string propertyName = GetPropertyName(property);
            string builtInName = BuiltInNames[property.PropertyType];
            bi.RosDefinition.Append(builtInName).Append(" ").AppendLine(propertyName);
            bi.BufferForMd5.Append(builtInName).Append(" ").Append(propertyName).Append("\n");
            bi.Fields.Add(new StructField<T, TField>(property));
        }

        static void InitializeArrayProperty<TField>(PropertyInfo property, BuilderInfo bi) where TField : unmanaged
        {
            string propertyName = GetPropertyName(property);
            string builtInName = BuiltInNames[typeof(TField)];
            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append(builtInName).Append("[] ").AppendLine(propertyName);
                bi.BufferForMd5.Append(builtInName).Append("[] ").Append(propertyName).Append("\n");
                bi.Fields.Add(new StructArrayField<T, TField>(property, propertyName));
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append(builtInName).Append("[").Append(size).Append("] ").AppendLine(propertyName);
                bi.BufferForMd5.Append(builtInName).Append("[").Append(size).Append("] ").Append(propertyName)
                    .Append("\n");
                bi.Fields.Add(new StructFixedArrayField<T, TField>(property, size, propertyName));
            }
        }

        static void InitializeListProperty<TField>(PropertyInfo property, BuilderInfo bi) where TField : unmanaged
        {
            string propertyName = GetPropertyName(property);
            string builtInName = BuiltInNames[typeof(TField)];
            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append(builtInName).Append("[] ").AppendLine(propertyName);
                bi.BufferForMd5.Append(builtInName).Append("[] ").Append(propertyName).Append("\n");
                bi.Fields.Add(new StructListField<T, TField>(property, propertyName));
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append(builtInName).Append("[").Append(size).Append("] ").AppendLine(propertyName);
                bi.BufferForMd5.Append(builtInName).Append("[").Append(size).Append("] ").Append(propertyName)
                    .Append("\n");
                bi.Fields.Add(new StructFixedListField<T, TField>(property, size, propertyName));
            }
        }

        [Preserve]
        static void InitializeMessageProperty<TField>(PropertyInfo property, BuilderInfo bi)
            where TField : struct, IMessage, IDeserializable<TField>
        {
            string propertyName = GetPropertyName(property);
            bi.RosDefinition.Append(BuiltIns.GetMessageType<TField>()).Append(" ").AppendLine(propertyName);
            bi.BufferForMd5.Append(BuiltIns.GetMd5Sum<TField>()).Append(" ").Append(propertyName).Append("\n");
            bi.Fields.Add(new MessageField<T, TField>(property, propertyName));
        }

        [Preserve]
        static void InitializeMessageArrayProperty<TField>(PropertyInfo property, BuilderInfo bi)
            where TField : struct, IMessage, IDeserializable<TField>
        {
            string propertyName = GetPropertyName(property);
            bi.BufferForMd5.Append(BuiltIns.GetMd5Sum<TField>()).Append(" ").Append(propertyName)
                .Append("\n"); // no [] here!
            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append(BuiltIns.GetMessageType<TField>()).Append("[] ").AppendLine(propertyName);
                bi.Fields.Add(new MessageArrayField<T, TField>(property, propertyName));
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append(BuiltIns.GetMessageType<TField>()).Append("[").Append(size).Append("] ")
                    .AppendLine(propertyName);
                bi.Fields.Add(new MessageFixedArrayField<T, TField>(property, size, propertyName));
            }
        }

        static void InitializeStringProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            bi.RosDefinition.Append("string ").AppendLine(propertyName);
            bi.BufferForMd5.Append("string ").Append(propertyName).Append("\n");
            bi.Fields.Add(new StringField<T>(property, propertyName));
        }

        static void InitializeStringArrayProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append("string[] ").AppendLine(propertyName);
                bi.BufferForMd5.Append("string[] ").Append(propertyName).Append("\n");
                bi.Fields.Add(new StringArrayField<T>(property, propertyName));
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append("string[").Append(size).Append("] ").AppendLine(propertyName);
                bi.BufferForMd5.Append("string[").Append(size).Append("] ").Append(propertyName).Append("\n");
                bi.Fields.Add(new StringFixedArrayField<T>(property, size, propertyName));
            }
        }

        static void InitializeStringListProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append("string[] ").AppendLine(propertyName);
                bi.BufferForMd5.Append("string[] ").Append(propertyName).Append("\n");
                bi.Fields.Add(new StringListField<T>(property, propertyName));
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append("string[").Append(size).Append("] ").AppendLine(propertyName);
                bi.BufferForMd5.Append("string[").Append(size).Append("] ").Append(propertyName).Append("\n");
                bi.Fields.Add(new StringFixedListField<T>(property, size, propertyName));
            }
        }

        static void InitializeMessageProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            bi.RosDefinition.Append(BuiltIns.GetMessageType(property.PropertyType)).Append(" ")
                .AppendLine(propertyName);
            bi.BufferForMd5.Append(BuiltIns.GetMd5Sum(property.PropertyType)).Append(" ").Append(propertyName)
                .Append("\n");

            Type fieldType = typeof(MessageField<,>).MakeGenericType(typeof(T), property.PropertyType);
            IMessageField<T> field = (IMessageField<T>)Activator.CreateInstance(fieldType, property, propertyName)!;
            bi.Fields.Add(field);
        }

        static void InitializeMessageArrayProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            Type elementType = property.PropertyType.GetElementType()!;
            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;

            bi.BufferForMd5.Append(BuiltIns.GetMd5Sum(elementType)).Append(" ").Append(propertyName)
                .Append("\n"); // no [] here!

            IMessageField<T> field;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append(BuiltIns.GetMessageType(elementType)).Append("[] ")
                    .AppendLine(propertyName);

                Type fieldType = typeof(MessageArrayField<,>).MakeGenericType(typeof(T), elementType);
                field = (IMessageField<T>)Activator.CreateInstance(fieldType, property, propertyName)!;
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append(BuiltIns.GetMessageType(elementType)).Append("[").Append(size).Append("] ")
                    .AppendLine(propertyName);
                Type fieldType = typeof(MessageFixedArrayField<,>).MakeGenericType(typeof(T), elementType);
                field = (IMessageField<T>)Activator.CreateInstance(fieldType, property, size, propertyName)!;
            }

            bi.Fields.Add(field);
        }

        static void InitializeEnumProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            Type underlyingType = Enum.GetUnderlyingType(property.PropertyType);
            string builtInName = BuiltInNames[underlyingType];

            bi.RosDefinition.Append(builtInName).Append(" ").AppendLine(propertyName);
            bi.BufferForMd5.Append(builtInName).Append(" ").Append(propertyName).Append("\n");

            Type structType = typeof(StructField<,>).MakeGenericType(typeof(T), underlyingType);
            IMessageField<T> field = (IMessageField<T>)Activator.CreateInstance(structType, property)!;
            bi.Fields.Add(field);
        }

        static void InitializeEnumArrayProperty(PropertyInfo property, BuilderInfo bi)
        {
            string propertyName = GetPropertyName(property);
            Type underlyingType = Enum.GetUnderlyingType(property.PropertyType);
            string builtInName = BuiltInNames[underlyingType];

            uint? fixedSize = property.GetCustomAttribute<FixedSizeArrayAttribute>()?.Size;
            IMessageField<T> field;
            if (fixedSize == null)
            {
                bi.RosDefinition.Append(builtInName).Append("[] ").AppendLine(propertyName);
                bi.BufferForMd5.Append(builtInName).Append("[] ").Append(propertyName).Append("\n");

                Type structType = typeof(StructArrayField<,>).MakeGenericType(typeof(T), underlyingType);
                field = (IMessageField<T>)Activator.CreateInstance(structType, property, propertyName)!;
            }
            else
            {
                uint size = fixedSize.Value;
                bi.RosDefinition.Append(builtInName).Append("[").Append(size).Append("] ").AppendLine(propertyName);
                bi.BufferForMd5.Append(builtInName).Append("[").Append(size).Append("] ").Append(propertyName)
                    .Append("\n");
                Type structType = typeof(StructFixedArrayField<,>).MakeGenericType(typeof(T), underlyingType);
                field = (IMessageField<T>)Activator.CreateInstance(structType, property, size, propertyName)!;
            }

            bi.Fields.Add(field);
        }


        static void InitializeConstant(BuilderInfo bi, FieldInfo field, string fieldType)
        {
            string entry = fieldType == "string"
                ? $"{fieldType} {GetPropertyName(field, false)}=\"{field.GetRawConstantValue()}\""
                : $"{fieldType} {GetPropertyName(field, false)}={field.GetRawConstantValue()}";

            bi.RosDefinition.AppendLine(entry);
            bi.BufferForMd5.AppendLine(entry);
        }

        static (string inputForMd5, string md5Hash) CreateMd5(BuilderInfo bi)
        {
            string inputForMd5 = bi.BufferForMd5.Length == 0
                ? ""
                : bi.BufferForMd5.ToString(0, bi.BufferForMd5.Length - 1); // remove trailing \n
            return (inputForMd5, RosWrapperBase.CreateMd5(inputForMd5));
        }

        static string CreateDependencies(string rosDefinition)
        {
            var rosDependencies = new StringBuilder(100);
            rosDependencies.Append(rosDefinition);

            var alreadyWritten = new HashSet<Type>();
            AddDependency(rosDependencies, typeof(T), alreadyWritten, true);

            return rosDependencies.ToString();
        }

        internal static string CreatePartialDependencies(HashSet<Type> alreadyWritten)
        {
            var rosDependencies = new StringBuilder(100);
            AddDependency(rosDependencies, typeof(T), alreadyWritten, true);
            return rosDependencies.ToString();
        }
    }
}