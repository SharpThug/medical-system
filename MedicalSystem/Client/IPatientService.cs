using Shared;

namespace Client
{
    public interface IPatientService
    {
        Task<ApiResponse<List<Patient>>> GetPatientsAsync(int count);
    }
}
