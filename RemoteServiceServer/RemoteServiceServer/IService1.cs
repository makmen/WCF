using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RemoteServiceServer
{
    [ServiceContract]
    public interface IRemoteAccess
    {
        [OperationContract]
        string[] RequestSql(string sql);

    }


}
