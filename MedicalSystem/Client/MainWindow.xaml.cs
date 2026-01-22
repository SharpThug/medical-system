using System.ComponentModel;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {

        private readonly LoginViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new LoginViewModel(null);
            }
        }

        public MainWindow(LoginViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginViewModel vm = (LoginViewModel)DataContext;

            string login = vm.Login;
            string password = PasswordBox.Password;


            try
            {
                var token = await vm.LoginAsync(login, password);
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
