using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCF_duplexServer
{

    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IDuplexSvc
    {
        [OperationContract(IsOneWay = true)]
        void ReturnTime(int period, int number);
    }
    /*это вторая часть контракта службы – контракт обратного вызова, метод ReceiveTime() описан на стороне клиента и может вызываться службой, при вызове служба передает клиенту информацию в string str*/

    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveTime(string str);
    }

    public class DuplexSvc : IDuplexSvc
    {
        /*метод ReturnTime() каждый раз запускается в новом потоке, для запуска потока используется делегат ParameterizedThreadStart, способный передать один параметер типа object, но поскольку нам надо передать в поток два параметра типа int, мы упаковываем их в List<int> и в таком виде передаем методу потока SendTimeToCLient() */
        public void ReturnTime(int period, int number)
        {
            DataValues src = new DataValues();
            src.callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            Thread t = new Thread(new ParameterizedThreadStart(src.SendTimeToCLient));
            t.IsBackground = true;
            List<int> parameters = new List<int>();
            parameters.Add(period);
            parameters.Add(number);
            t.Start(parameters);
        }
    }

    /*метод SendTimeToCLient() будет вызван после обращения клиента к нашей службе. Он подождет до начала следующей минуты и начнет отправлять клиенту сообщения с периодичностью period миллисекунд, всего таких сообщений он отправит number. Каждое из этих сообщений доставляется клиенту методом ReceiveTime() */

    public class DataValues
    {
        //член типа контракта обратного вызова
        public IClientCallback callback = null;

        public void SendTimeToCLient(object data)
        {
            //по условию задачи надо подождать до начала следующей минуты
            int s = 60 - DateTime.Now.Second;
            Thread.Sleep(1000);
            DateTime start = DateTime.Now;
            //достать из object data наши два параметра типа int,
            List<int> parameters = (List<int>)data;
            int period = parameters[0];
            int number = parameters[1];
            //каждое сообщение клиенту готовится в цикле
            for (int i = 0; i < number; i++)
            {
                try
                {
                    //задержка между сообщениями period секунд
                    Thread.Sleep(period * 1000);
                    TimeSpan result = DateTime.Now - start;
                    TimeSpan r = result.Add(new TimeSpan(0, 0, s));
                    callback.ReceiveTime(DateTime.Now.ToLongTimeString().ToString() +
                    " время работы со службой - " +
                    r.Minutes.ToString() + ":" +
                    r.Seconds.ToString());
                }
                catch (Exception ex)
                {
                }
            }
        }
    }

    class Program
    {


        static void Main(string[] args)
        {
            ServiceHost sh = new ServiceHost(typeof(DuplexSvc));
            sh.Open();
            Console.WriteLine("Для завершения нажмите <ENTER>\n");
            Console.ReadLine();
            sh.Close();
        }
    }
}
