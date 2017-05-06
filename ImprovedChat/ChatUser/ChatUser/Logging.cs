using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatUser
{
    public class Logging
    {
        private static Logging instance;
        private string fileLog = "../../common.log";

        private Logging()
        {

        }

        public static Logging GetInstance()
        {
            if (instance == null)
            {
                instance = new Logging();
            }

            return instance;
        }

        public void Write(string msg) 
        {
            using (FileStream fsWriter = new FileStream(
                fileLog,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None)
                )
            {
                DateTime.Now.ToString();
                byte[] info = new UTF8Encoding(true).GetBytes( "[" + DateTime.Now.ToString() + "] " + msg + "\r\n");
                fsWriter.Write(info, 0, info.Length);
            }
        }

    }
}
