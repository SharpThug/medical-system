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
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Patient>> GetPatientsAsync(int count)
        {
            try
            {
                string apiUrl = $"https://localhost:7218/api/patient?count={count}";

                // Парсим ApiResponse<List<Patient>>
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<Patient>>>(apiUrl);

                if (response == null || !response.Success)
                    return new List<Patient>();

                return response.Data ?? new List<Patient>();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении пациентов с API: " + ex.Message, ex);
            }
        }
    }
}