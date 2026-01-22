using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginViewModel vm = (LoginViewModel)DataContext;

            string login = vm.Login;
            string password = PasswordBox.Password;

            await vm.LoginAsync(login, password);
        }
    }
}
