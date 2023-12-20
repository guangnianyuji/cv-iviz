/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [DataContract]
    public sealed class JointConstraint : IDeserializable<JointConstraint>, IMessage
    {
        // Constrain the position of a joint to be within a certain bound
        [DataMember (Name = "joint_name")] public string JointName;
        // the bound to be achieved is [position - tolerance_below, position + tolerance_above]
        [DataMember (Name = "position")] public double Position;
        [DataMember (Name = "tolerance_above")] public double ToleranceAbove;
        [DataMember (Name = "tolerance_below")] public double ToleranceBelow;
        // A weighting factor for this constraint (denotes relative importance to other constraints. Closer to zero means less important)
        [DataMember (Name = "weight")] public double Weight;
    
        /// Constructor for empty message.
        public JointConstraint()
        {
            JointName = "";
        }
        
        /// Constructor with buffer.
        public JointConstraint(ref ReadBuffer b)
        {
            b.DeserializeString(out JointName);
            b.Deserialize(out Position);
            b.Deserialize(out ToleranceAbove);
            b.Deserialize(out ToleranceBelow);
            b.Deserialize(out Weight);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new JointConstraint(ref b);
        
        public JointConstraint RosDeserialize(ref ReadBuffer b) => new JointConstraint(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(JointName);
            b.Serialize(Position);
            b.Serialize(ToleranceAbove);
            b.Serialize(ToleranceBelow);
            b.Serialize(Weight);
        }
        
        public void RosValidate()
        {
            if (JointName is null) BuiltIns.ThrowNullReference();
        }
    
        public int RosMessageLength => 36 + BuiltIns.GetStringSize(JointName);
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "moveit_msgs/JointConstraint";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "c02a15146bec0ce13564807805b008f0";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE21Qy07DMBC8+ytG6qUI6AlxR/0MhKqNs2kWOd7KXhKJr2eTgqkQB1/mtTPe4ai5WiHJ" +
                "sJFx0SommqEDCO8q2WCKjrGIjS4iRC62yjv9yH1wr+TzVXnKNHEIuy1po7+9FEfhmXtIxWs78ehs4kI5" +
                "8qnjpMvD7/n7G446nfktDEnJnp+apgF/lP/gW/pa7AULy3m0tfJA0bRg8OfLKuLPRxj2PWc1riicyGRm" +
                "yHRRn+1h6yT1geXGUA84Jq2OOfnJRTEx5YrEtTar3bVm1xLhC72gSgx9AQAA";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
