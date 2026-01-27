using Shared;

namespace Api
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetLastPatientsAsync(int count);
    }
}
