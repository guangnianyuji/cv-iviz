using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [DataContract]
    public sealed class CheckIfRobotStateExistsInWarehouse : IService
    {
        /// Request message.
        [DataMember] public CheckIfRobotStateExistsInWarehouseRequest Request { get; set; }
        
        /// Response message.
        [DataMember] public CheckIfRobotStateExistsInWarehouseResponse Response { get; set; }
        
        /// Empty constructor.
        public CheckIfRobotStateExistsInWarehouse()
        {
            Request = new CheckIfRobotStateExistsInWarehouseRequest();
            Response = new CheckIfRobotStateExistsInWarehouseResponse();
        }
        
        /// Setter constructor.
        public CheckIfRobotStateExistsInWarehouse(CheckIfRobotStateExistsInWarehouseRequest request)
        {
            Request = request;
            Response = new CheckIfRobotStateExistsInWarehouseResponse();
        }
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (CheckIfRobotStateExistsInWarehouseRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (CheckIfRobotStateExistsInWarehouseResponse)value;
        }
        
        public const string ServiceType = "moveit_msgs/CheckIfRobotStateExistsInWarehouse";
        public string RosServiceType => ServiceType;
        
        public string RosMd5Sum => "3b47364b81dd44f643fa67c9cd127486";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class CheckIfRobotStateExistsInWarehouseRequest : IRequest<CheckIfRobotStateExistsInWarehouse, CheckIfRobotStateExistsInWarehouseResponse>, IDeserializable<CheckIfRobotStateExistsInWarehouseRequest>
    {
        [DataMember (Name = "name")] public string Name;
        [DataMember (Name = "robot")] public string Robot;
    
        /// Constructor for empty message.
        public CheckIfRobotStateExistsInWarehouseRequest()
        {
            Name = "";
            Robot = "";
        }
        
        /// Explicit constructor.
        public CheckIfRobotStateExistsInWarehouseRequest(string Name, string Robot)
        {
            this.Name = Name;
            this.Robot = Robot;
        }
        
        /// Constructor with buffer.
        public CheckIfRobotStateExistsInWarehouseRequest(ref ReadBuffer b)
        {
            b.DeserializeString(out Name);
            b.DeserializeString(out Robot);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new CheckIfRobotStateExistsInWarehouseRequest(ref b);
        
        public CheckIfRobotStateExistsInWarehouseRequest RosDeserialize(ref ReadBuffer b) => new CheckIfRobotStateExistsInWarehouseRequest(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(Name);
            b.Serialize(Robot);
        }
        
        public void RosValidate()
        {
            if (Name is null) BuiltIns.ThrowNullReference();
            if (Robot is null) BuiltIns.ThrowNullReference();
        }
    
        public int RosMessageLength => 8 + BuiltIns.GetStringSize(Name) + BuiltIns.GetStringSize(Robot);
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class CheckIfRobotStateExistsInWarehouseResponse : IResponse, IDeserializable<CheckIfRobotStateExistsInWarehouseResponse>
    {
        [DataMember (Name = "exists")] public bool Exists;
    
        /// Constructor for empty message.
        public CheckIfRobotStateExistsInWarehouseResponse()
        {
        }
        
        /// Explicit constructor.
        public CheckIfRobotStateExistsInWarehouseResponse(bool Exists)
        {
            this.Exists = Exists;
        }
        
        /// Constructor with buffer.
        public CheckIfRobotStateExistsInWarehouseResponse(ref ReadBuffer b)
        {
            b.Deserialize(out Exists);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new CheckIfRobotStateExistsInWarehouseResponse(ref b);
        
        public CheckIfRobotStateExistsInWarehouseResponse RosDeserialize(ref ReadBuffer b) => new CheckIfRobotStateExistsInWarehouseResponse(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(Exists);
        }
        
        public void RosValidate()
        {
        }
    
        /// <summary> Constant size of this message. </summary> 
        public const int RosFixedMessageLength = 1;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        public override string ToString() => Extensions.ToString(this);
    }
}
