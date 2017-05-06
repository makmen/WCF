using RemoteServiceClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteServiceClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RemoteAccessClient service;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                service = new RemoteAccessClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                service = null;
            }
        }

        private void btnSql_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorControls.NotEmptyTextBox(tbRequest))
            {
                string[] result = null;
                try 
                {
                    result = service.RequestSql(tbRequest.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (result != null)
                {
                    HandlerMsg(result);
                }
                else
                {
                    lvResult.Items.Clear();
                    MessageBox.Show("Ошибка в запросе");
                }
            }
        }

        public void HandlerMsg(string[] result)
        {
            List<string[]> listRows = new List<string[]>();
            string[] strArray = null;
            for (int i = 0; i < result.Length; i++)
            {
                strArray = result[i].Split('\t');
                listRows.Add(strArray);
            }
            if (listRows.Count > 0)
            {
                ShowSqlGrid(listRows);
                lvResult.Items.Clear();
            }
        }

        public void AddGridColumn(GridView myGridView, string header, string name)
        {
            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding(name);
            gvc1.Header = header;
            gvc1.Width = 100;
            myGridView.Columns.Add(gvc1);
        }

        public string DefineUnicName(string[] names, string need)
        {
            string result = need;
            int i = 0, j = 0;
            while (i < names.Length)
            {
                if (names[i] != result)
                {
                    ++i;
                }
                else
                {
                    result = need;
                    result += "_" + ++j;
                    i = 0;
                }
            }

            return result;
        }

        public void ShowSqlGrid(List<string[]> listRows)
        {
            lvResult.Dispatcher.BeginInvoke(new Action(delegate()
            {
                lvResult.Items.Clear();
                GridView myGridView = new GridView();
                myGridView.AllowsColumnReorder = true;
                myGridView.ColumnHeaderToolTip = "Result";
                int index = 0;
                string[] names = new string[listRows[index].Length];
                for (int j = 0; j < listRows[index].Length; j++)
                {
                    AddGridColumn(myGridView, listRows[index][j], listRows[index][j]);
                    names[j] = DefineUnicName(names, listRows[index][j]);
                }
                listRows.Remove(listRows[index]);
                lvResult.HorizontalAlignment = HorizontalAlignment.Center;
                lvResult.BorderBrush = Brushes.White;
                lvResult.View = myGridView;
                for (int i = 0; i < listRows.Count; i++)
                {
                    dynamic row = new ExpandoObject();
                    for (int j = 0; j < listRows[i].Length; j++)
                    {
                        ((IDictionary<String, Object>)row).Add(names[j], (object)listRows[i][j]);
                    }
                    lvResult.Items.Add(row);
                }
            }));
        }
    }
}
