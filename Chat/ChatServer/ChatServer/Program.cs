using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatServer.model;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost sh = new ServiceHost(typeof(Connection));
            sh.Open();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            Console.ReadLine();
            sh.Close();
        }
    }
}
