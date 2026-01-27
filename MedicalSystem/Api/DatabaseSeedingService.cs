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

            await SeedPatientsAsync();

            Console.WriteLine("Данные вставлены в базу, включая пользователя Ivanova.");
        }

        private async Task SeedPatientsAsync()
        {
            var patients = new[]
            {
        new
        {
            CardNumber = "P0001",
            OmsNumber = "1234567890123456",
            LastName = "Петров",
            FirstName = "Алексей",
            Patronymic = "Сергеевич",
            BirthDate = new DateTime(1985, 4, 12),
            Gender = "M",
            Phone = "+7 900 123 45 67",
            Address = "г. Москва, ул. Ленина, д.1",
            Email = "petrov@example.com",
            Allergies = "Нет",
            ChronicDiseases = "Гипертония"
        },
        new
        {
            CardNumber = "P0002",
            OmsNumber = "2345678901234567",
            LastName = "Иванова",
            FirstName = "Мария",
            Patronymic = "Игоревна",
            BirthDate = new DateTime(1990, 9, 5),
            Gender = "F",
            Phone = "+7 900 234 56 78",
            Address = "г. Санкт-Петербург, ул. Пушкина, д.5",
            Email = "ivanova@example.com",
            Allergies = "Пенициллин",
            ChronicDiseases = "Астма"
        },
        new
        {
            CardNumber = "P0003",
            OmsNumber = (string?)null,
            LastName = "Смирнов",
            FirstName = "Дмитрий",
            Patronymic = "Александрович",
            BirthDate = new DateTime(2000, 12, 21),
            Gender = "M",
            Phone = "+7 900 345 67 89",
            Address = "г. Казань, ул. Баумана, д.10",
            Email = "smirnov@example.com",
            Allergies = "Нет",
            ChronicDiseases = "Нет"
        }
    };

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(
                """
        IF NOT EXISTS (
            SELECT 1
            FROM Patients
            WHERE CardNumber = @CardNumber
        )
        INSERT INTO Patients
            (CardNumber, OmsNumber, LastName, FirstName, Patronymic, BirthDate, Gender, Phone, Address, Email, Allergies, ChronicDiseases)
        VALUES
            (@CardNumber, @OmsNumber, @LastName, @FirstName, @Patronymic, @BirthDate, @Gender, @Phone, @Address, @Email, @Allergies, @ChronicDiseases);
        """,
                conn);

            foreach (var patient in patients)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 10).Value = patient.CardNumber;
                cmd.Parameters.Add("@OmsNumber", SqlDbType.NVarChar, 16).Value = (object?)patient.OmsNumber ?? DBNull.Value;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = patient.LastName;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = patient.FirstName;
                cmd.Parameters.Add("@Patronymic", SqlDbType.NVarChar, 50).Value = (object?)patient.Patronymic ?? DBNull.Value;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = patient.BirthDate;
                cmd.Parameters.Add("@Gender", SqlDbType.NChar, 1).Value = patient.Gender;
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)patient.Phone ?? DBNull.Value;
                cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 50).Value = (object?)patient.Address ?? DBNull.Value;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = (object?)patient.Email ?? DBNull.Value;
                cmd.Parameters.Add("@Allergies", SqlDbType.NVarChar, 500).Value = (object?)patient.Allergies ?? DBNull.Value;
                cmd.Parameters.Add("@ChronicDiseases", SqlDbType.NVarChar, 500).Value = (object?)patient.ChronicDiseases ?? DBNull.Value;

                await cmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Пациенты успешно добавлены в базу.");
        }
    }
}
