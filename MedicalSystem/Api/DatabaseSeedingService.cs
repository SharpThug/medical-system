using Microsoft.Data.SqlClient;
using System.Data;
using BCrypt.Net;

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

            using (SqlCommand cmd = new SqlCommand(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Departments
                    WHERE Code = @Code
                )
                INSERT INTO Departments (Name, Code, Type)
                VALUES (@Name, @Code, @Type);
                """,
                conn))
            {
                foreach (var dept in departments)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = dept.Name;
                    cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 20).Value = dept.Code;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = dept.Type;
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            string plainPassword = "12345";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            using (SqlCommand cmd = new SqlCommand(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Users
                    WHERE Login = @Login
                )
                INSERT INTO Users 
                    (Login, Password, LastName, FirstName, Patronymic, Role, Specialty, DepartmentId, IsActive)
                VALUES 
                    (@Login, @Password, @LastName, @FirstName, @Patronymic, @Role, @Specialty, @DepartmentId, 1);
                """,
                conn))
                        {
                cmd.Parameters.Add("@Login", SqlDbType.NVarChar, 50).Value = "Ivanova";
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = hashedPassword;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = "Иванова";
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = "Ирина";
                cmd.Parameters.Add("@Patronymic", SqlDbType.NVarChar, 50).Value = "Сергеевна";
                cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = "Doctor";
                cmd.Parameters.Add("@Specialty", SqlDbType.NVarChar, 50).Value = "Офтальмолог";
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = 1;

                await cmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Данные вставлены в базу, включая пользователя Ivanova.");
        }
    }
}
