using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RemoteServiceServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RemoteAccess : IRemoteAccess
    {
        private DbLink link = new DbLink();

        public string[] RequestSql(string sql)
        {
            string[] result = null;
            DbDataReader dataReader = link.Select(sql);
            result = handler(dataReader);
            link.Close();

            return result;
        }

        public string[] handler(DbDataReader dataReader)
        {
            string[] nameColumn = null, result = null;
            List<string> listRows = new List<string>();
            if (dataReader != null)
            {
                nameColumn = new string[dataReader.FieldCount];
                for (int i = 0; i < nameColumn.Length; i++)
                {
                    nameColumn[i] = dataReader.GetName(i);
                }
                string temp = string.Join("\t", nameColumn);
                listRows.Add(temp);
                while (dataReader.Read())
                {
                    string[] rows = new string[dataReader.FieldCount];
                    for (int i = 0; i < nameColumn.Length; i++)
                    {
                        rows[i] = dataReader[nameColumn[i]].ToString();
                    }
                    temp = string.Join("\t", rows);
                    listRows.Add(temp);
                }
            }
            if (listRows.Count > 0)
            {
                result = new string[listRows.Count];
                for (int i = 0; i < listRows.Count; i++)
                {
                    result[i] = listRows[i];
                }
            }

            return result;
        }
    }
}
