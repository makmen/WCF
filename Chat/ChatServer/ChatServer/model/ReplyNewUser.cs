using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.model
{
    [DataContract]
    public class ReplyNewUser
    {
        [DataMember]
        public User User;
        [DataMember]
        public User[] Users;
    }
}
