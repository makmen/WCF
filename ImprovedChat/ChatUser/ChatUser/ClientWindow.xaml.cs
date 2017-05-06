using ChatUser.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatUser
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private MainWindow parent;
        private User user;
        private string ban = "Images/ban.png";
        private string avatar = "Images/ok.png";

        public List<User> PrivateDenied {get; set;}

        private static Logging log;
        public static Logging Log
        {
            get { return ClientWindow.log; }
            set { ClientWindow.log = value; }
        }

        private List<User> listClients;
        public List<User> ListClients
        {
            get { return listClients; }
        }

        private bool statusAllMassDenied = false;
        public bool StatusAllMassDenied
        {
            get { return statusAllMassDenied; }
            set { statusAllMassDenied = value; }
        }

        private bool statusAllDenied = false;
        public bool StatusAllDenied
        {
            get { return statusAllDenied; }
            set { statusAllDenied = value; }
        }

        private Dictionary<string, string> smiles;
        private string additionalTeg = "::";

        public Dictionary<string, string> Smiles
        {
            get { return smiles; }
            set { smiles = value; }
        }

        public ClientWindow(ReplyNewUser newUser)
        {
            InitializeComponent();
            log = Logging.GetInstance();
            this.Title = newUser.User.Name;
            this.user = newUser.User;
            listClients = newUser.Users.ToList();
            MyMessages(newUser.User.Name + " вошел в чат");
            PrivateDenied = new List<User>();
            // add smiles
            DirectoryInfo di = new DirectoryInfo("../../Smiles");
            DirectoryInfo[] resultDirectory = di.GetDirectories();
            DirectoryInfo[] dirs = di.GetDirectories();
            FileInfo[] files = di.GetFiles();
            smiles = new Dictionary<string, string>();
            foreach (FileInfo file in files)
            {
                smiles.Add(additionalTeg + System.IO.Path.GetFileNameWithoutExtension(file.FullName) + additionalTeg, file.FullName);
            }
            foreach (var item in smiles)
            {
                DrawSmile(item);
            }
        }

        private void DrawSmile(KeyValuePair<string, string> item)
        {
            BitmapImage logo = new BitmapImage(new Uri(item.Value, UriKind.Absolute));
            Image image = new Image { Source = logo };
            image.Width = 30;
            image.Height = 30;
            image.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
            var inlineUiContainer = new InlineUIContainer(image);
            inlineUiContainer.BaselineAlignment = BaselineAlignment.Center;
            tbSmiles.Inlines.Add(inlineUiContainer);
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

        public void MyMessages(string message)
        {
            log.Write(message);
            AddTextToResult(message, 1);
        }

        public void AddTextToResult(string message, int param = 0)
        {
            TextRange textRange = new TextRange(tbResult.Document.ContentEnd, tbResult.Document.ContentEnd);
            int pos = 0;
            foreach (Match match in Regex.Matches(message, @"::(.+?)::",
                RegexOptions.IgnoreCase))
            {
                textRange = new TextRange(tbResult.Document.ContentEnd, tbResult.Document.ContentEnd);
                textRange.Text += (message).Substring(pos, (match.Index - pos));
                pos = match.Index;
                if (smiles.ContainsKey(match.Value))
                {
                    ShowImage(smiles[match.Value]);
                }
                else
                {
                    textRange.Text += match.Value;
                }
                pos += match.Value.Length;
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, (param == 0) ? Brushes.White : Brushes.Green);
            }
            textRange = new TextRange(tbResult.Document.ContentEnd, tbResult.Document.ContentEnd);
            textRange.Text += (pos > 0) ?
                (message).Substring(pos, ((message).Length - pos)) :
                message;
            textRange.Text += "\r";
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, (param == 0) ? Brushes.White : Brushes.Green);
        }

        private void ShowImage(string img)
        {
            Image image = new Image();
            image.Stretch = Stretch.None;
            BitmapImage bmp = new BitmapImage(new Uri(img, UriKind.Relative));
            image.Source = bmp;

            tbResult.BeginChange();
            TextRange textRange = new TextRange(tbResult.Document.ContentEnd, tbResult.Document.ContentEnd);
            InlineUIContainer imageContainer = new InlineUIContainer(image, textRange.Start);
            tbResult.EndChange();
        }

        public void UpdateListClients()
        {
            lstClients.ItemsSource = null;
            //lstClients.DisplayMemberPath = "Name";
            lstClients.ItemsSource = listClients;
        }

        private void btnSendPrivate_Click(object sender, RoutedEventArgs e)
        {
            User[] userToSend = new User[lstClients.SelectedItems.Count];
            int i = 0;
            foreach (User user in lstClients.SelectedItems)
            {
                userToSend[i++] = user;
            }
            if (userToSend.Length > 0)
            {
                Message msg = new Message();
                msg.From = user;
                msg.To = userToSend;
                if (ErrorControls.NotEmptyTextBox(tbMsg))
                {
                    msg.Msg = tbMsg.Text;
                    try
                    {
                        parent.Proxy.SendPrivateMessage(msg);
                        MyMessages(user.Name + ": " + tbMsg.Text);
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
                    parent.Proxy.SendMessageToAll(user.Name + ": " + tbMsg.Text, this.user);
                    MyMessages(user.Name + ": " + tbMsg.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                tbMsg.Text = "";
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bttClear_Click(object sender, RoutedEventArgs e)
        {
            tbResult.Document.Blocks.Clear();
        }

        private void btnAllMassDenied_Click(object sender, RoutedEventArgs e)
        {
            statusAllMassDenied = ((bool)btnAllMassDenied.IsChecked) ? true : false;
        }

        private void btnAllDenied_Click(object sender, RoutedEventArgs e)
        {
            statusAllDenied = ((bool)btnAllDenied.IsChecked) ? true : false;
        }

        private void btnPrivateDenied_Click(object sender, RoutedEventArgs e)
        {
            int index;
            foreach (User user in lstClients.SelectedItems)
            {
                if (!PrivateDenied.Contains(user))
                {
                    PrivateDenied.Add(user);
                    index = listClients.IndexOf(listClients.FirstOrDefault(c => c.Id == user.Id));
                    listClients[index].Avatar = ban;
                }
            }
            index = lstClients.SelectedItems.Count;
            UpdateListClients();
            if (index == 0)
            {
                MessageBox.Show("Выберите пользователя");
            }
        }

        private void btnCancelPrivateDenied_Click(object sender, RoutedEventArgs e)
        {
            int index;
            foreach (User user in lstClients.SelectedItems)
            {
                if (PrivateDenied.Contains(user))
                {
                    PrivateDenied.Remove(user);
                    index = listClients.IndexOf(listClients.FirstOrDefault(c => c.Id == user.Id));
                    listClients[index].Avatar = avatar;
                }
            }
            index = lstClients.SelectedItems.Count;
            UpdateListClients();
            if (index == 0)
            {
                MessageBox.Show("Выберите пользователя");
            }
        }

        private void btnSmile_Click(object sender, RoutedEventArgs e)
        {
            popSmile.IsOpen = true;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            string str = System.IO.Path.GetFileNameWithoutExtension(image.Source.ToString());
            tbMsg.Text += additionalTeg + str + additionalTeg;
            tbMsg.SelectionStart = tbMsg.Text.Length;
            popSmile.IsOpen = false;
        }
    }
}
