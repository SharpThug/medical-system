namespace Api
{
    public interface IPatientService
    {
        public Task<List<Patient>> GetLastPatientsAsync(int count);
    }
}
