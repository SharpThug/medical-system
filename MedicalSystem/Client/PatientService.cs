using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using Shared;

namespace Client
{
    public class PatientService : IPatientService
    {
        private readonly HttpClient _httpClient;

        public PatientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<List<Patient>>> GetPatientsAsync(int count)
        {
            string apiUrl = $"api/patient?count={count}";
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"Ошибка сети или API: {httpEx.Message}. Проверьте доступность API.");
            }

            ApiResponse<List<Patient>>? result = await response.Content.ReadFromJsonAsync<ApiResponse<List<Patient>>>();

            return result ?? throw new InvalidOperationException("Ответ от сервера пуст или невалиден");
        }
    }
}