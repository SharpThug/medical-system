using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Client
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _login = string.Empty;

        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        public Task LoginAsync(string login, string password)
        {
            var request = new LoginRequest
            {
                Login = login,
                Password = password
            };

            //вызов апишки

            return Task.CompletedTask;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
