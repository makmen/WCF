using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPDualClientConsole.ServiceReference1;

namespace TCPDualClientConsole
{
    public class CallbackHandler : IHelloCallback
    {
        public void HelloFromServer()
        {
            Console.WriteLine("HelloFromServer");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InstanceContext site = new InstanceContext(new CallbackHandler());
                HelloClient proxy = new HelloClient(site);
                Console.WriteLine("Client");
                proxy.HelloFromClient();

                Console.ReadLine();
                proxy.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
