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


            ApiResponse<string> response = await _authService.LoginAsync(login, password);

            if (response.Success)
            {
                Session.Token = response.Data!;

                var mainAppWindow = _serviceProvider.GetRequiredService<MainAppWindow>();
                mainAppWindow.Show();
                this.Close();
            }
            else
            {
                //обработаешь ошибку
            }
        }
    }
}

