using ChatClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class CallbackHandler : IConnectionCallback
    {
        private ClientWindow client;

        public ClientWindow Client
        {
            get { return client; }
            set { client = value; }
        }

        public void SendAllFromServer(string message)
        {
            client.AddTextToResult(message);
        }

        public void AddUserToAll(User user)
        {
            client.ListClients.Add(user);
            client.UpdateListClients();
        }

        public void RemoveClient(User removedUser)
        {
            client.ListClients.Remove(client.ListClients.FirstOrDefault(c => c.Id == removedUser.Id));
            client.UpdateListClients();
        }

        public void SendPrivate(string message)
        {
            client.AddTextToResult(message);
        }
    }
}
