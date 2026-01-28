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
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<string>> LoginAsync(string login, string password)
        {
            LoginRequest request = new LoginRequest
            {
                Login = login,
                Password = password
            };

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"Ошибка сети или API: {httpEx.Message}. Проверьте доступность API.");//потом сделаешь в глобальный handler отлов ошибок
            }

            ApiResponse<string>? result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            return result ?? throw new InvalidOperationException("Ответ от сервера пуст или невалиден");
        }
    }
}