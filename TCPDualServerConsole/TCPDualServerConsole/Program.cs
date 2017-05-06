using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TCPDualServerConsole
{

    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IHello
    {
        [OperationContract(IsOneWay = true)]
        void HelloFromClient();

    }

    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void HelloFromServer();
    }

    public class Hello : IHello
    {
        public static IClientCallback callback;

        public void HelloFromClient()
        {
            Console.WriteLine("HelloFromClient");
            callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            callback.HelloFromServer();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost sh = new ServiceHost(typeof(Hello));
            sh.Open();
            Console.WriteLine("Для завершения нажмите <ENTER>\n");
            Console.ReadLine();
            sh.Close();
        }
    }
}
