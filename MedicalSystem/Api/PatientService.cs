using Shared;

namespace Api
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<List<Patient>> GetLastPatientsAsync(int count)
        {
            return await _patientRepository.GetLastPatientsAsync(count);
        }
    }
}
