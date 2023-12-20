/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [DataContract]
    public sealed class OrientedBoundingBox : IDeserializable<OrientedBoundingBox>, IMessage
    {
        // the pose of the box
        [DataMember (Name = "pose")] public GeometryMsgs.Pose Pose;
        // the extents of the box, assuming the center is at the origin
        [DataMember (Name = "extents")] public GeometryMsgs.Point32 Extents;
    
        /// Constructor for empty message.
        public OrientedBoundingBox()
        {
        }
        
        /// Explicit constructor.
        public OrientedBoundingBox(in GeometryMsgs.Pose Pose, in GeometryMsgs.Point32 Extents)
        {
            this.Pose = Pose;
            this.Extents = Extents;
        }
        
        /// Constructor with buffer.
        public OrientedBoundingBox(ref ReadBuffer b)
        {
            b.Deserialize(out Pose);
            b.Deserialize(out Extents);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new OrientedBoundingBox(ref b);
        
        public OrientedBoundingBox RosDeserialize(ref ReadBuffer b) => new OrientedBoundingBox(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(in Pose);
            b.Serialize(in Extents);
        }
        
        public void RosValidate()
        {
        }
    
        /// <summary> Constant size of this message. </summary> 
        public const int RosFixedMessageLength = 68;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "moveit_msgs/OrientedBoundingBox";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "da3bd98e7cb14efa4141367a9d886ee7";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE71UTUvDQBC9768Y8KJQKrTiQfAgHsSDoOhdtsk0GUx24s7Gpv31zm4+aulBEGlOk52P" +
                "fe/NzJ5BKBEaFgReJ3vFnSmQawx++15LIZfP0RtDjDlLIdgFdEF+ZMzAirQ1uSKdZOpGDyRgQzpgTwW5" +
                "o7rkwnIxljPm9p8/8/T6cAPHZJTGHXhsPIpebAOxi1ySCuRg7RFBGpvhDDKu43E++CnFWpdHRmPuHEyi" +
                "MgWYl9Yqf5fq7uNORVChKMO3UuXPWO8mJ2OXaeRq9S9CPqBr1hXbcH0F3WRtJ2t3Gvh76UYOU6N0mg70" +
                "PAQf/z73uq/Z13PzC6PR2pywNcvF35pzvqFQgq7LivrVU1UyEk25mGvFxwBJKx3YGnMd2MDQ6jz3k7kp" +
                "0eOXrqReI7SqoloS0Ka5HmDNAbTOXvVUyeWD1BILNp5rDjFZZeYGvV1RRWGbUsfMGkVsgTElR6HC9WCC" +
                "/UBoG6jUPXRMUTnQzub6bmh2xQOxiCc9HeziDlqJSqR3xfbvlO0x31fc5vHuvpGqTTdZ28namW/1kU6A" +
                "5QQAAA==";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
