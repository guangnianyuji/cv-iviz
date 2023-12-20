/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.RosbridgeMsgs
{
    [DataContract]
    public sealed class ConnectedClient : IDeserializable<ConnectedClient>, IMessage
    {
        [DataMember (Name = "ip_address")] public string IpAddress;
        [DataMember (Name = "connection_time")] public time ConnectionTime;
    
        /// Constructor for empty message.
        public ConnectedClient()
        {
            IpAddress = "";
        }
        
        /// Explicit constructor.
        public ConnectedClient(string IpAddress, time ConnectionTime)
        {
            this.IpAddress = IpAddress;
            this.ConnectionTime = ConnectionTime;
        }
        
        /// Constructor with buffer.
        public ConnectedClient(ref ReadBuffer b)
        {
            b.DeserializeString(out IpAddress);
            b.Deserialize(out ConnectionTime);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new ConnectedClient(ref b);
        
        public ConnectedClient RosDeserialize(ref ReadBuffer b) => new ConnectedClient(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(IpAddress);
            b.Serialize(ConnectionTime);
        }
        
        public void RosValidate()
        {
            if (IpAddress is null) BuiltIns.ThrowNullReference();
        }
    
        public int RosMessageLength => 12 + BuiltIns.GetStringSize(IpAddress);
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "rosbridge_msgs/ConnectedClient";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "7f2187ce389b39b2b3bb2a3957e54c04";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAEysuKcrMS1fILIhPTEkpSi0u5irJzE1VSM7Py0tNLsnMz4sH8bm4AFvLoaQoAAAA";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
