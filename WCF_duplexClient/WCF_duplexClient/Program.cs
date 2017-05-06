using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCF_duplexClient.ServiceReference1;

namespace WCF_duplexClient
{
    public class CallbackHandler : IDuplexSvcCallback
    {
        static InstanceContext site = new InstanceContext(new CallbackHandler());
        public static DuplexSvcClient proxy = new DuplexSvcClient(site);

        public void ReceiveTime(string str)
        {
            Console.WriteLine("Получено сообщение :\n" + str);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CallbackHandler.proxy.ReturnTime(2, 5);
            Console.ReadKey();

        }
    }
}
