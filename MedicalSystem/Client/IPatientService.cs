namespace Client
{
    public interface IPatientService
    {
        public Task<List<Patient>> GetPatientsAsync(int count);
    }
}
