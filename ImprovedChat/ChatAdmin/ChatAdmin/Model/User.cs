using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ChatAdmin.Model
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Id;
        [DataMember]
        public string Name;
        [DataMember]
        public string Avatar;
        public OperationContext Context;
    }
}
