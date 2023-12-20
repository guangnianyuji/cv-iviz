/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.ObjectRecognitionMsgs
{
    [DataContract]
    public sealed class ObjectType : IDeserializable<ObjectType>, IMessage
    {
        //################################################# OBJECT ID #########################################################
        // Contains information about the type of a found object. Those two sets of parameters together uniquely define an
        // object
        // The key of the found object: the unique identifier in the given db
        [DataMember (Name = "key")] public string Key;
        // The db parameters stored as a JSON/compressed YAML string. An object id does not make sense without the corresponding
        // database. E.g., in object_recognition, it can look like: "{'type':'CouchDB', 'root':'http://localhost'}"
        // There is no conventional format for those parameters and it's nice to keep that flexibility.
        // The object_recognition_core as a generic DB type that can read those fields
        // Current examples:
        // For CouchDB:
        //   type: 'CouchDB'
        //   root: 'http://localhost:5984'
        //   collection: 'object_recognition'
        // For SQL household database:
        //   type: 'SqlHousehold'
        //   host: 'wgs36'
        //   port: 5432
        //   user: 'willow'
        //   password: 'willow'
        //   name: 'household_objects'
        //   module: 'tabletop'
        [DataMember (Name = "db")] public string Db;
    
        /// Constructor for empty message.
        public ObjectType()
        {
            Key = "";
            Db = "";
        }
        
        /// Explicit constructor.
        public ObjectType(string Key, string Db)
        {
            this.Key = Key;
            this.Db = Db;
        }
        
        /// Constructor with buffer.
        public ObjectType(ref ReadBuffer b)
        {
            b.DeserializeString(out Key);
            b.DeserializeString(out Db);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new ObjectType(ref b);
        
        public ObjectType RosDeserialize(ref ReadBuffer b) => new ObjectType(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(Key);
            b.Serialize(Db);
        }
        
        public void RosValidate()
        {
            if (Key is null) BuiltIns.ThrowNullReference();
            if (Db is null) BuiltIns.ThrowNullReference();
        }
    
        public int RosMessageLength => 8 + BuiltIns.GetStringSize(Key) + BuiltIns.GetStringSize(Db);
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "object_recognition_msgs/ObjectType";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "ac757ec5be1998b0167e7efcda79e3cf";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE5VSwW7bMAy95yuI5uBL4QBrO2y+NUmHrehWDM1lp0C2GFuLLLoS3TQY9u+jLLvI2lN1" +
                "MfxEvvf4qPn8vQful7c3qw18W8O7e6czm81hRY6VcQGM25FvFRtyoErqGbhB4GOHQDtQsKPeaaDyN1ac" +
                "w6ahILcHgoAcYkWnvGqR0QdgqlGaPfTOPPZoj6BxZxyCcqKYKKL2RgT2eIzdUetUoRiQ1A9Go2OzM8Jo" +
                "3HBRmyd0oMtZYG9cHVkmQl2eWglMHjWoIBPcPtz/WFTUdh5DEPDX9fc7SAQ5XLtRWdRAEwZwxNCqPcqE" +
                "TmY9GG6mVCrywtGR09IrulqxKlXAHG7yOj+PLhPZ1mNFtTMxVYEZKuXAEu3Bmj0WcPYniwlnRbaivmrW" +
                "y+wcMk/EgjTMXbFYWKqUlbQ5+3uWRvSSSLQnNtxTTIacspC2Fz9iMS7nJAUluRrOpMlUsjWSvLCTslhv" +
                "8dmUxho+5mOCb61vZWBMIdbo0JsK1sv0NgaSOJVHpUdl2ZTVIT6uXnJyDPis2s5iKAT7IgbHYeMvDDQF" +
                "vAQwYDECwV5HUFx9/nSZKiqyVlyKOal76zgblR5+3oGsLWBDVr/s6T/hh0f7dapI3IMUZIc6XHxMSEde" +
                "kKvLiw/Dr1T7WGCspcNYoUI4kNevYCcriINMAttkNaTblnRv473YssjUZdODlqc9+wcQ+4upFQQAAA==";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
