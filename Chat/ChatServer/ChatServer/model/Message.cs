using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ChatServer.model
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public User From;
        [DataMember]
        public User To;
        [DataMember]
        public string Msg;
    }
}
