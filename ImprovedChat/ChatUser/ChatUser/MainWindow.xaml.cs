using ChatUser.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace ChatUser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InstanceContext site;
        private ConnectionClient proxy;
        public ConnectionClient Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        private CallbackHandler handler;
        public CallbackHandler Handler
        {
            get { return handler; }
            set { handler = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool check = ErrorControls.NotEmptyTextBox(tbName);
            if (check)
            {
                ReplyNewUser newUser = null;
                try
                {
                    // connect to the server ... 
                    handler = new CallbackHandler();
                    site = new InstanceContext(handler);
                    proxy = new ConnectionClient(site);
                    // logging
                    newUser = proxy.Join(tbName.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (newUser != null)
                {
                    ClientWindow client = new ClientWindow(newUser);
                    client.Owner = this;
                    client.Show();
                    this.Hide();
                }
            }
        }
    }
}
