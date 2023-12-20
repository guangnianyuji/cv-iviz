/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.VisionMsgs
{
    [DataContract]
    public sealed class BoundingBox2D : IDeserializable<BoundingBox2D>, IMessage
    {
        // A 2D bounding box that can be rotated about its center.
        // All dimensions are in pixels, but represented using floating-point
        //   values to allow sub-pixel precision. If an exact pixel crop is required
        //   for a rotated bounding box, it can be calculated using Bresenham's line
        //   algorithm.
        // The 2D position (in pixels) and orientation of the bounding box center.
        [DataMember (Name = "center")] public GeometryMsgs.Pose2D Center;
        // The size (in pixels) of the bounding box surrounding the object relative
        //   to the pose of its center.
        [DataMember (Name = "size_x")] public double SizeX;
        [DataMember (Name = "size_y")] public double SizeY;
    
        /// Constructor for empty message.
        public BoundingBox2D()
        {
            Center = new GeometryMsgs.Pose2D();
        }
        
        /// Explicit constructor.
        public BoundingBox2D(GeometryMsgs.Pose2D Center, double SizeX, double SizeY)
        {
            this.Center = Center;
            this.SizeX = SizeX;
            this.SizeY = SizeY;
        }
        
        /// Constructor with buffer.
        public BoundingBox2D(ref ReadBuffer b)
        {
            Center = new GeometryMsgs.Pose2D(ref b);
            b.Deserialize(out SizeX);
            b.Deserialize(out SizeY);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new BoundingBox2D(ref b);
        
        public BoundingBox2D RosDeserialize(ref ReadBuffer b) => new BoundingBox2D(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            Center.RosSerialize(ref b);
            b.Serialize(SizeX);
            b.Serialize(SizeY);
        }
        
        public void RosValidate()
        {
            if (Center is null) BuiltIns.ThrowNullReference();
            Center.RosValidate();
        }
    
        /// <summary> Constant size of this message. </summary> 
        public const int RosFixedMessageLength = 40;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "vision_msgs/BoundingBox2D";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "9ab41e2a4c8627735e5091a9abd68b02";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE61UTWsUQRC9768oyCEKyQoqHgQPkYDkIATUc6iZqdlt7elu+2N3x1/vq+7MJIs5urAw" +
                "/VFVr9571Rd0Q29vqfPFDcbt8HGivOdMPTvqhKLPnGUgxo1MJifqxWWJ280F3VhLg5nEJeNdIo5CxlEw" +
                "J7HpijrcjxKiJA0YqCTNP1rPGR/XwRuXkYTowLZIouyJrfVHSqW7rkkIwb3R5Fu6GwmA5MR9bhWojz6Q" +
                "Sajxu5goQ801+ki8gn7e1RXAL031bPti+QnV54pyz9NlImuc1Fxsdz6avJ+2G6y/70WJCj6ZDET0am31" +
                "NZANhKvok+uZH8GhnJO60LYTP0mO88OUdunNvU+CrO1wKZPMHznL/1K+VGJc1nrou5/SK+PoyxxaB6BU" +
                "j4BZNMdz9aoOH97XWg+n8+W82Xz6z7/N129fPtJLvQPorbqkVzmwuLfCgFvwV+xjgcneVd6l6nDnkMZJ" +
                "ZEu+RPTb+wkWHBrzpvpIg3kNXU24iiMHiXPeK3UqnZoGIqQAt42mJw7Bmr7eTjTxr4YEu3BcNMAJY3pl" +
                "u16AjR9ptuykJsPKxNVl9ZZOgw/ZTPD4TEdjq7uTxEPLDpzGIXZqIIcSFR3q9JLUoq33kY5Ce0ZMYDBg" +
                "dQx8MJgedIUWwALnOWBdW6Qfd82bqBDhpiBq7lYdlFEJ5ESahXzLOxQQa9QjI/dtKoP1+RllW7pB6OzL" +
                "WZ4jBoUcLnZSae2sLEqgtey9TQvPutdD4woSklVzz5d4PSBlsJIFDOFRMMMVcdJKl0MDh4RRjN6B4voa" +
                "tbKDGUeJdceFApPXnkvWb7wcGUOtyEBZNqpIRP49x2FtLq+zjTmJFUbJTZeZjySYtLgIS2pbmqAK76RW" +
                "UnKlvn5so/AwP7arZ9Z0kaMKtD6qg7rFNDbQG4StY48dOakllBh+emf+eVuwpWAndmb0dkD8MrxPYzyv" +
                "XwCXebP5C9+IYLLnBQAA";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
