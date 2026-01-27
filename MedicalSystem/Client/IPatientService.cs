using Shared;

namespace Client
{
    public interface IPatientService
    {
        Task<List<Patient>> GetPatientsAsync(int count);
    }
}
