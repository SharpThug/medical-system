using Microsoft.Data.SqlClient;
using System.Data;

namespace Api
{

    public class DatabaseSeedingService : IDatabaseSeeder
    {
        private readonly string _connectionString;

        public DatabaseSeedingService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SeedDataAsync()
        {
            var departments = new[]
            {
                new { Name = "Офтальмология", Code = "OFT", Type = "Хирургическое" },
                new { Name = "Терапия", Code = "TER", Type = "Терапевтическое" }
            };

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();


            using SqlCommand cmd = new SqlCommand(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Departments
                    WHERE Code = @Code
                )
                INSERT INTO Departments (Name, Code, Type)
                VALUES (@Name, @Code, @Type);
                """,
                conn
            );

            foreach (var dept in departments)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = dept.Name;
                cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 20).Value = dept.Code;
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = dept.Type;
                await cmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Данные вставлены в базу.");
        }
    }
}
