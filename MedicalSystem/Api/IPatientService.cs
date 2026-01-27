namespace Api
{
    public interface IPatientService
    {
        Task<List<Patient>> GetLastPatientsAsync(int count);
    }
}
