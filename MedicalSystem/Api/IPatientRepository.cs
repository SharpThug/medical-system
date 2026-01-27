namespace Api
{
    public interface IPatientRepository
    {
        public Task<List<Patient>> GetLastPatientsAsync(int count);
    }
}
