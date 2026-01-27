using System.ComponentModel;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using Shared;

namespace Client
{
    public partial class MainWindow : Window
    {
        private readonly IAuthService _authService;

        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IAuthService authService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = PasswordBox.Password;

            try
            {
                string token = await _authService.LoginAsync(login, password);
                Session.Token = token;

                var mainAppWindow = _serviceProvider.GetRequiredService<MainAppWindow>();
                mainAppWindow.Show();
                    this.Close();



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }
        }
    }
}
