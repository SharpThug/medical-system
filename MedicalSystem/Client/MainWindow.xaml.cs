using Shared;
using System.ComponentModel;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        private readonly IAuthService _authService;

        public MainWindow(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = PasswordBox.Password;

            try
            {
                string token = await _authService.LoginAsync(login, password);
                Session.Token = token;

                var mainAppWindow = new MainAppWindow();
                mainAppWindow.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }
        }
    }
}
