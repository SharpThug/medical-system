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

            string sql = @"
        SELECT TOP (@Count) 
               CardNumber, 
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
    ";

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);

            // Параметры всегда лучше явно указывать тип
            command.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = count;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var patient = new Patient
                {
                    CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    Patronymic = reader.IsDBNull(reader.GetOrdinal("Patronymic"))
                                 ? null
                                 : reader.GetString(reader.GetOrdinal("Patronymic")),
                    BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                    Phone = reader.IsDBNull(reader.GetOrdinal("Phone"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Phone")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                              ? null
                              : reader.GetString(reader.GetOrdinal("Address")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Email")),
                    Allergies = reader.IsDBNull(reader.GetOrdinal("Allergies"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Allergies")),
                    ChronicDiseases = reader.IsDBNull(reader.GetOrdinal("ChronicDiseases"))
                                      ? null
                                      : reader.GetString(reader.GetOrdinal("ChronicDiseases")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                };

                patients.Add(patient);
            }
            return patients;
        }
    }
}

