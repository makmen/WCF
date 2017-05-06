using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ChatClient.ServiceReference1;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private MainWindow parent;
        private User user;

        private List<User> listClients;
        public List<User> ListClients
        {
            get { return listClients; }
        }

        public ClientWindow(ReplyNewUser newUser)
        {
            InitializeComponent();
            this.Title = newUser.User.Name;
            this.user = newUser.User;
            listClients = newUser.Users.ToList();
            UpdateListClients();
            AddTextToResult(newUser.User.Name + " вошел в чат");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parent = (MainWindow)this.Owner;
            parent.Handler.Client = this;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            parent.Close();
            try
            {
                parent.Proxy.Exit(user);
                parent.Proxy.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AddTextToResult(string message)
        {
            tbResult.Text += message + "\r\n";
        }

        public void UpdateListClients()
        {
            lstClients.ItemsSource = null;
            lstClients.DisplayMemberPath = "Name";
            lstClients.ItemsSource = listClients;
        }

        private void btnSendPrivate_Click(object sender, RoutedEventArgs e)
        {
            User userTo = (User)lstClients.SelectedItem;
            if (userTo != null)
            {
                Message msg = new Message();
                msg.From = user;
                msg.To = userTo;
                if (ErrorControls.NotEmptyTextBox(tbMsg))
                {
                    msg.Msg = tbMsg.Text;
                    AddTextToResult(user.Name + ": " + tbMsg.Text);
                    try
                    {
                        parent.Proxy.SendPrivateMessage(msg);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    tbMsg.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя");
            }
        }

        private void btnSendAll_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorControls.NotEmptyTextBox(tbMsg))
            {
                try
                {
                    parent.Proxy.SendMessageToAll(user.Name + ": " + tbMsg.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                tbMsg.Text = "";
            }
        }
    }
}
