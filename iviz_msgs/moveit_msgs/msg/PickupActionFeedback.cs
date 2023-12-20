/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [DataContract]
    public sealed class PickupActionFeedback : IDeserializable<PickupActionFeedback>, IActionFeedback<PickupFeedback>
    {
        [DataMember (Name = "header")] public StdMsgs.Header Header { get; set; }
        [DataMember (Name = "status")] public ActionlibMsgs.GoalStatus Status { get; set; }
        [DataMember (Name = "feedback")] public PickupFeedback Feedback { get; set; }
    
        /// Constructor for empty message.
        public PickupActionFeedback()
        {
            Status = new ActionlibMsgs.GoalStatus();
            Feedback = new PickupFeedback();
        }
        
        /// Explicit constructor.
        public PickupActionFeedback(in StdMsgs.Header Header, ActionlibMsgs.GoalStatus Status, PickupFeedback Feedback)
        {
            this.Header = Header;
            this.Status = Status;
            this.Feedback = Feedback;
        }
        
        /// Constructor with buffer.
        public PickupActionFeedback(ref ReadBuffer b)
        {
            Header = new StdMsgs.Header(ref b);
            Status = new ActionlibMsgs.GoalStatus(ref b);
            Feedback = new PickupFeedback(ref b);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new PickupActionFeedback(ref b);
        
        public PickupActionFeedback RosDeserialize(ref ReadBuffer b) => new PickupActionFeedback(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            Header.RosSerialize(ref b);
            Status.RosSerialize(ref b);
            Feedback.RosSerialize(ref b);
        }
        
        public void RosValidate()
        {
            if (Status is null) BuiltIns.ThrowNullReference();
            Status.RosValidate();
            if (Feedback is null) BuiltIns.ThrowNullReference();
            Feedback.RosValidate();
        }
    
        public int RosMessageLength
        {
            get {
                int size = 0;
                size += Header.RosMessageLength;
                size += Status.RosMessageLength;
                size += Feedback.RosMessageLength;
                return size;
            }
        }
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "moveit_msgs/PickupActionFeedback";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "12232ef97486c7962f264c105aae2958";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE71W23LbNhB951dgRg+xO7XaJr2kntGDKimOOk6isdW+ekBwRaIhQRUXyfr7ngUpinKs" +
                "Rg9JNLJ1A84enD272LckM7KiiC+JVF7XptTpQ+Vy98NNLct7L31wwsWXZKHVx7B+Q5SlUn0Uq/ZNMvrC" +
                "j+Td/c01gmYNkbcNvYEAG5NJm4mKvMykl2JVg73OC7JXJW2oZKbVmjIRf/W7NbkhNi4L7QSeORmysix3" +
                "Ijgs8rVQdVUFo5X0JLyu6Gg/dmojpFhL67UKpbRYX9tMG16+srIiRsfT0b+BjCIxn15jjXGkgtcgtAOC" +
                "siSdNjl+FEnQxr96yRuSwXJbX+Ej5chBF1z4QnomS49rS455SneNGN81hxsCG+IQomROXMTvHvDRXQoE" +
                "AQVa16oQF2C+2PmiNgAksZFWy7QkBlZQAKgveNOLyx6yidBGmnoP3yAeYpwDazpcPtNVgZyVfHoXcgiI" +
                "hWtbb3SGpekugqhSk/ECxrPS7hLe1YRMBm9YYyzCrpgRvErnaqWRgExstS8S5y2jx2w86Cz5Sm48WRwJ" +
                "v0Vmc7xwfE7w633FNB8Ws/fT+fsbsX+MxI/4z7akuE0U0okdeTZkSqyPahLfCtTERs7tBnXQYI4ny/nf" +
                "M9HD/OkYkzMSrIWyMGFKrNFZwIu72ezdYjmbdsAvj4EtKYK1YUukHPbgb+B+54VceThZez695QTRY6wD" +
                "kyfifx4D/MEkUYXGcKjKdUmMoL3bo4DoxZJsheoruRV4umwp3/81mcxm0x7lV8eUt0CWqtDEtF1QrMIq" +
                "cB94TohTYcZ/fLg76MJhfn4mTFrHo2ch2vLA/dlIWaDPSsOucDXKYCV1GSydonc3+3M26fEbiV8+pWfp" +
                "H1L+hANiQdXBP7XL95/nmJKS6KkRswsW0Ce9BFPuEOjU2mxkqbNTB2id11XKSPz6DZzXWc/UPhbhwXxd" +
                "8jqFJ+Pb20Mlj8Rv5xJMCVcVPcvwHHWRk0+zdUzarLSt+FLj68P3u0BkQtnRIfo2ef0FDnGezGyKo/Jr" +
                "AvC1ccITtx/ul32okfg9Ao7NXoz29gCSyJA1BqFGBNlJwCjDZgpwMHiZRd3SM2rPMXbNarOkW43jo3Kk" +
                "edI6k8G4LOttnEd4IUrBct12lxXItBcV15jozVa8JaM05DnL2C7y9OiTb3iVzadJ44BmBGlFcp7TzeeJ" +
                "dzIk3RYas0W8j3stJbqDMp6F5nF0Ce0d81Qn7CfD/sEpybFAGHGoWiNXZYndjOma5G0JoTvovfVgSbLc" +
                "UiKj/qjQ8kd3accLtGLQ2x1nYT+yshuxA/NVKD3GSedkTk1q3JqUXmm1L4bIwA1bdJ71mgUgVYVYFOhz" +
                "GquG++TxEPKVUlfBito3eTuexhGyDc+jByX/ATV586PbCwAA";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
