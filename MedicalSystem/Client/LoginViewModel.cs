using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _login;
        private string _password;
        private bool _isBusy;

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsBusy);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            IsBusy = true;

            try
            {
                using var client = new HttpClient();
                var loginData = new { login = this.Login, password = this.Password };

                string apiUrl = "https://your-api.com/auth/login";

                var response = await client.PostAsJsonAsync(apiUrl, loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    MessageBox.Show($"Успешно! Токен: {result?.Token}");
                }
                else
                {
                    MessageBox.Show("Ошибка авторизации. Проверьте логин и пароль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
