using System.Runtime.Serialization;
using System.Xml;
using Iviz.Msgs;

namespace Iviz.Urdf
{
    [DataContract]
    public sealed class Box
    {
        [DataMember] public Vector3f Size { get; }

        internal Box(XmlNode node)
        {
            Size = Vector3f.Parse(node.Attributes?["size"], Vector3f.One);
        }
        
        public override string ToString() => BuiltIns.ToJsonString(this);
    }
}