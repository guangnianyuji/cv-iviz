using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [DataContract]
    public sealed class SaveRobotStateToWarehouse : IService
    {
        /// Request message.
        [DataMember] public SaveRobotStateToWarehouseRequest Request { get; set; }
        
        /// Response message.
        [DataMember] public SaveRobotStateToWarehouseResponse Response { get; set; }
        
        /// Empty constructor.
        public SaveRobotStateToWarehouse()
        {
            Request = new SaveRobotStateToWarehouseRequest();
            Response = new SaveRobotStateToWarehouseResponse();
        }
        
        /// Setter constructor.
        public SaveRobotStateToWarehouse(SaveRobotStateToWarehouseRequest request)
        {
            Request = request;
            Response = new SaveRobotStateToWarehouseResponse();
        }
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (SaveRobotStateToWarehouseRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (SaveRobotStateToWarehouseResponse)value;
        }
        
        public const string ServiceType = "moveit_msgs/SaveRobotStateToWarehouse";
        public string RosServiceType => ServiceType;
        
        public string RosMd5Sum => "555cb479e433361a8f0a29f1cd7f3ad2";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class SaveRobotStateToWarehouseRequest : IRequest<SaveRobotStateToWarehouse, SaveRobotStateToWarehouseResponse>, IDeserializable<SaveRobotStateToWarehouseRequest>
    {
        [DataMember (Name = "name")] public string Name;
        [DataMember (Name = "robot")] public string Robot;
        [DataMember (Name = "state")] public MoveitMsgs.RobotState State;
    
        /// Constructor for empty message.
        public SaveRobotStateToWarehouseRequest()
        {
            Name = "";
            Robot = "";
            State = new MoveitMsgs.RobotState();
        }
        
        /// Explicit constructor.
        public SaveRobotStateToWarehouseRequest(string Name, string Robot, MoveitMsgs.RobotState State)
        {
            this.Name = Name;
            this.Robot = Robot;
            this.State = State;
        }
        
        /// Constructor with buffer.
        public SaveRobotStateToWarehouseRequest(ref ReadBuffer b)
        {
            b.DeserializeString(out Name);
            b.DeserializeString(out Robot);
            State = new MoveitMsgs.RobotState(ref b);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new SaveRobotStateToWarehouseRequest(ref b);
        
        public SaveRobotStateToWarehouseRequest RosDeserialize(ref ReadBuffer b) => new SaveRobotStateToWarehouseRequest(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(Name);
            b.Serialize(Robot);
            State.RosSerialize(ref b);
        }
        
        public void RosValidate()
        {
            if (Name is null) BuiltIns.ThrowNullReference();
            if (Robot is null) BuiltIns.ThrowNullReference();
            if (State is null) BuiltIns.ThrowNullReference();
            State.RosValidate();
        }
    
        public int RosMessageLength
        {
            get {
                int size = 8;
                size += BuiltIns.GetStringSize(Name);
                size += BuiltIns.GetStringSize(Robot);
                size += State.RosMessageLength;
                return size;
            }
        }
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class SaveRobotStateToWarehouseResponse : IResponse, IDeserializable<SaveRobotStateToWarehouseResponse>
    {
        [DataMember (Name = "success")] public bool Success;
    
        /// Constructor for empty message.
        public SaveRobotStateToWarehouseResponse()
        {
        }
        
        /// Explicit constructor.
        public SaveRobotStateToWarehouseResponse(bool Success)
        {
            this.Success = Success;
        }
        
        /// Constructor with buffer.
        public SaveRobotStateToWarehouseResponse(ref ReadBuffer b)
        {
            b.Deserialize(out Success);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new SaveRobotStateToWarehouseResponse(ref b);
        
        public SaveRobotStateToWarehouseResponse RosDeserialize(ref ReadBuffer b) => new SaveRobotStateToWarehouseResponse(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(Success);
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
