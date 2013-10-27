using System.Windows;
using System.Windows.Controls;

namespace DayzServerManager
{
    /// <summary>
    /// Interaction logic for FirstPage.xaml
    /// </summary>
    public partial class FirstPage : UserControl
    {
        public FirstPage()
        {
            InitializeComponent();
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            int port;
            if (string.IsNullOrEmpty(_textServer.Text) || string.IsNullOrEmpty(_textPort.Text) ||
                !int.TryParse(_textPort.Text, out port) || string.IsNullOrEmpty(_textDatabase.Text) ||
                string.IsNullOrEmpty(_textUser.Text) || string.IsNullOrEmpty(_textPassword.Text))
                MessageBox.Show("Invalid params", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!Context.ConnectToDB(_textServer.Text, port, _textDatabase.Text, _textUser.Text, _textPassword.Text))
                MessageBox.Show("Can't connect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
