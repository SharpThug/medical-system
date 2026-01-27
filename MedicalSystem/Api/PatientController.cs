using Microsoft.AspNetCore.Mvc;

using Shared;

namespace Api
{
    [ApiController]
    [Route("api/patient")]
    [ValidateModel]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLastPatients([FromQuery] int count)
        {
            var patients = await _patientService.GetLastPatientsAsync(count);

            return Ok(new ApiResponse<List<Patient>>(true, patients, null));
        }
    }
}
