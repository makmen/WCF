using ChatAdmin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChatAdmin
{
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IConnection
    {
        [OperationContract]
        ReplyNewUser Join(string userName);

        [OperationContract(IsOneWay = true)]
        void SendPrivateMessage(Message message);

        [OperationContract(IsOneWay = true)]
        void SendMessageToAll(string message, User user = null);

        [OperationContract(IsOneWay = true)]
        void Exit(User user);
    }

    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendAllFromServer(string message, User From);

        [OperationContract(IsOneWay = true)]
        void AddUserToAll(User user);

        [OperationContract(IsOneWay = true)]
        void RemoveClient(User user);

        [OperationContract(IsOneWay = true)]
        void SendPrivate(string message, User From);
    }
}
