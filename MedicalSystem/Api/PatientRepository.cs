using Shared;

using Microsoft.Data.SqlClient;

namespace Api
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Patient>> GetLastPatientsAsync(int count)
        {
            Console.WriteLine($"Попытка вытянуть последних {count} пациентов");

            var patients = new List<Patient>();

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(
                """
                SELECT TOP (@Count) 
                    Id,
                    CardNumber, 
                    OmsNumber,
                    LastName, 
                    FirstName, 
                    Patronymic, 
                    BirthDate, 
                    Phone, 
                    Gender, 
                    Address, 
                    Email, 
                    Allergies, 
                    ChronicDiseases, 
                    CreatedDate
                FROM Patients
                ORDER BY CreatedDate DESC
                """, 
                connection);

            command.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = count;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var patient = new Patient
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                    OmsNumber = reader["OmsNumber"] as string,
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    Patronymic = reader.GetString(reader.GetOrdinal("Patronymic")),
                    BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    Phone = reader["Phone"] as string,
                    Address = reader["Address"] as string,
                    Email = reader["Email"] as string,
                    Allergies = reader["Allergies"] as string,
                    ChronicDiseases = reader["ChronicDiseases"] as string,
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                };
                patients.Add(patient);
            }
            return patients;
        }
    }
}

