using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IPatientService
    {
        public Task<List<Patient>> GetPatientsAsync(int count);
    }
}
