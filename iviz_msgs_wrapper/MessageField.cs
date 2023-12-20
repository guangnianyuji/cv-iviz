using System;
using System.Reflection;
using Iviz.Msgs;

namespace Iviz.MsgsWrapper
{
    internal sealed class MessageField<T, TField> : IMessageField<T> 
        where T : IDeserializable<T>, ISerializable   
        where TField : IDeserializable<TField>, ISerializable, new()
    {
        static readonly IDeserializable<TField> Generator = new TField();
        static readonly bool IsValueType = typeof(T).IsValueType;
        static readonly int? FieldSize = BuiltIns.TryGetFixedSize<TField>(out int realFieldSize) ? realFieldSize : null;

        readonly Func<T, TField> getter;
        readonly Action<T, TField> setter;
        readonly string propertyName;

        public MessageField(PropertyInfo property, string propertyName)
        {
            getter = (Func<T, TField>) Delegate.CreateDelegate(typeof(Func<T, TField>), property.GetGetMethod()!);
            setter = (Action<T, TField>) Delegate.CreateDelegate(typeof(Action<T, TField>),
                property.GetSetMethod()!);
            this.propertyName = propertyName;
        }

        public void RosSerialize(T msg, ref WriteBuffer b) => getter(msg).RosSerialize(ref b);
        public void RosDeserialize(T msg, ref ReadBuffer b) => setter(msg, Generator.RosDeserialize(ref b));
        public int RosLength(T msg) => FieldSize ?? getter(msg).RosMessageLength;

        public void RosValidate(T msg)
        {
            if (IsValueType)
            {
                return;
            }

            var value = getter(msg);
            if (value is null)
            {
                throw new NullReferenceException($"Field '{propertyName}' is null");
            }
            
            value.RosValidate();
        }
    }
}