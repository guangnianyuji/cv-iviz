using System;
using System.Reflection;
using Iviz.Msgs;

namespace Iviz.MsgsWrapper
{
    internal sealed class StringFixedArrayField<T> : IMessageField<T> where T : IDeserializable<T>, ISerializable
    {
        readonly Func<T, string[]> getter;
        readonly Action<T, string[]> setter;
        readonly uint arraySize;
        readonly string propertyName;

        public StringFixedArrayField(PropertyInfo property, uint arraySize, string propertyName)
        {
            this.propertyName = propertyName;
            this.arraySize = arraySize;
            getter = (Func<T, string[]>) Delegate.CreateDelegate(typeof(Func<T, string[]>),
                property.GetGetMethod()!);
            setter = (Action<T, string[]>) Delegate.CreateDelegate(typeof(Action<T, string[]>),
                property.GetSetMethod()!);
        }

        public void RosSerialize(T msg, ref WriteBuffer b) => b.SerializeArray(getter(msg), (int) arraySize);
        public void RosDeserialize(T msg, ref ReadBuffer b) => setter(msg, b.DeserializeStringArray((int) arraySize));

        public int RosLength(T msg)
        {
            string[] array = getter(msg);
            int count = 4 * array.Length;
            foreach (var s in array)
            {
                count += BuiltIns.UTF8.GetByteCount(s);
            }

            return count;
        }

        public void RosValidate(T msg)
        {
            string[] array = getter(msg);
            if (array is null)
            {
                throw new NullReferenceException($"Field '{propertyName}' is null");
            }

            if (array.Length != arraySize)
            {
                throw new RosInvalidSizeForFixedArrayException(propertyName, array.Length, (int) arraySize);
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is null) throw new NullReferenceException($"{propertyName}[{i}]");
            }
        }
    }
}