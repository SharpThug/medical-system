using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using Shared;

namespace Client
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    Login = login,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    return result?.Token ?? string.Empty;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка авторизации: {response.StatusCode}. {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"Ошибка сети: {httpEx.Message}. Проверьте доступность API по адресу https://localhost:7218");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при авторизации: {ex.Message}");
            }
        }
    }
}