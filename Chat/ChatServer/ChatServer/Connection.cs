using ChatServer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Connection : IConnection
    {
        static private List<User> users = new List<User>(); // all connections to the server

        public ReplyNewUser Join(string userName)
        {
            User user = new User() { 
                Id = Guid.NewGuid().ToString(), 
                Name = userName,
                Context = OperationContext.Current
            };
            // send new client list of all users of the chat 
            ReplyNewUser newUserReply = new ReplyNewUser();
            newUserReply.User = user;
            newUserReply.Users = users.ToArray();
            // update new list of users
            AddNewUserToAll(user);
            // send message that the new user came
            SendMessageToAll(user.Name + " вошел в чат");
            // add the new user in the list
            users.Add(user);

            return newUserReply;
        }

        public void SendMessageToAll(string message)
        {
            foreach (User user in users)
            {
                user.Context.GetCallbackChannel<IClientCallback>().SendAllFromServer(message);
            }
        }

        public void SendPrivateMessage(Message message)
        {
            if (message.From != null &&
                message.To != null &&
                message.Msg != "")
            {
                User userTo = users.FirstOrDefault(c => c.Id == message.To.Id);
                userTo.Context.GetCallbackChannel<IClientCallback>().SendPrivate(message.From.Name + ": " + message.Msg);
            }
        }

        public void Exit(User removedUser)
        {
            // delete client from List<User> users
            users.Remove(users.FirstOrDefault(c => c.Id == removedUser.Id));
            // send message that new user has gone
            SendMessageToAll(removedUser.Name + " вышел из чата");
            // update new list
            foreach (User user in users)
            {
                user.Context.GetCallbackChannel<IClientCallback>().RemoveClient(removedUser);
            }
        }

        public void AddNewUserToAll(User newUser)
        {
            foreach (User user in users)
            {
                user.Context.GetCallbackChannel<IClientCallback>().AddUserToAll(newUser);
            }
        }
    }
}
