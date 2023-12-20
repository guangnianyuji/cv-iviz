using System.Runtime.InteropServices;
using Iviz.Msgs;

namespace Iviz.MsgsGen.Dynamic;

public sealed class StructField<T> : IField<T> where T : unmanaged
{
    public T Value { get; set; }
        
    public FieldType Type => FieldType.Struct;

    object IField.Value => Value;

    public int RosLength => Marshal.SizeOf<T>();

    public void RosValidate()
    {
    }

    public void RosSerialize(ref WriteBuffer b)
    {
        b.Serialize(Value);
    }

    public void RosDeserializeInPlace(ref ReadBuffer b)
    {
        b.Deserialize(out T t);
        Value = t;
    }

    public IField Generate()
    {
        return new StructField<T>();
    }
}