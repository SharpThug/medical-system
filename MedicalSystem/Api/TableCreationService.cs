using System.Text.RegularExpressions;

using Microsoft.Data.SqlClient;

namespace Api
{
    public class TableCreationService : ITableCreationService
    {
        private readonly string _connectionString;

        public TableCreationService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateAllTablesAsync()
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await CreateTableIfNotExistsAsync(conn, "Patients", """
                CREATE TABLE Patients (
                    Id BIGINT PRIMARY KEY IDENTITY(1,1),
                    CardNumber NVARCHAR(10) UNIQUE NOT NULL,
                    OmsNumber NVARCHAR(16) UNIQUE NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    FirstName NVARCHAR(50) NOT NULL,
                    Patronymic NVARCHAR(50),
                    BirthDate DATE NOT NULL,
                    Gender NCHAR(1) NOT NULL,
                    Phone NVARCHAR(20),
                    Address NVARCHAR(50),
                    Email NVARCHAR(50),
                    Allergies NVARCHAR(500),
                    ChronicDiseases NVARCHAR(500),
                    CreatedDate DATE DEFAULT GETDATE()
                );
            """);

            await CreateTableIfNotExistsAsync(conn, "Departments", """
                CREATE TABLE Departments (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(50) UNIQUE NOT NULL,
                    Code NVARCHAR(20) UNIQUE NOT NULL,
                    Type NVARCHAR(50) NOT NULL,
                    HeadDoctorId INT,
                    IsActive BIT DEFAULT 1
                );
            """);

            await CreateTableIfNotExistsAsync(conn, "Users", """
                CREATE TABLE Users (
                    Id BIGINT PRIMARY KEY IDENTITY(1,1),
                    Login NVARCHAR(50) UNIQUE NOT NULL,
                    Password NVARCHAR(255) NOT NULL,
                    LastName NVARCHAR(50),
                    FirstName NVARCHAR(50),
                    Patronymic NVARCHAR(50),
                    Role NVARCHAR(50) NOT NULL,
                    Specialty NVARCHAR(50),
                    DepartmentId INT NOT NULL,
                    IsActive BIT DEFAULT 1,
                    CONSTRAINT FK_Users_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
                );
            """);

            await CreateTableIfNotExistsAsync(conn, "Diagnoses", """
                CREATE TABLE Diagnoses (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Code NVARCHAR(10) NOT NULL,
                    Name NVARCHAR(50) NOT NULL,
                    Category NVARCHAR(50) NOT NULL,
                    Description NVARCHAR(500)
                );
            """);

            await CreateTableIfNotExistsAsync(conn, "Visits", """
                CREATE TABLE Visits (
                    Id BIGINT PRIMARY KEY IDENTITY(1,1),
                    PatientId BIGINT NOT NULL,
                    DoctorId BIGINT NOT NULL,
                    Type NVARCHAR(50) NOT NULL,
                    Complaints NVARCHAR(500),
                    Anamnesis NVARCHAR(500),
                    ObjectiveStatus NVARCHAR(500),
                    DiagnosisId INT,
                    TreatmentPlan NVARCHAR(500),
                    Recommendations NVARCHAR(500),
                    NextVisitDate DATE,
                    Date DATETIME2 DEFAULT GETDATE(),
                    CONSTRAINT FK_Visits_Patients FOREIGN KEY (PatientId) REFERENCES Patients(Id),
                    CONSTRAINT FK_Visits_Users FOREIGN KEY (DoctorId) REFERENCES Users(Id),
                    CONSTRAINT FK_Visits_Diagnoses FOREIGN KEY (DiagnosisId) REFERENCES Diagnoses(Id)
                );
            """);

            await CreateTableIfNotExistsAsync(conn, "Treatments", """
                CREATE TABLE Treatments (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    VisitId BIGINT NOT NULL,
                    Type NVARCHAR(50),
                    Dosage NVARCHAR(100),
                    Frequency NVARCHAR(100),
                    Duration NVARCHAR(100),
                    Instructions NVARCHAR(500),
                    Eye NCHAR(2),
                    StartDate DATE,
                    EndDate DATE,
                    CONSTRAINT FK_Treatments_Visits FOREIGN KEY (VisitId) REFERENCES Visits(Id)
                );
            """);

            await CreateTableIfNotExistsAsync(conn, "SavedFilters", """
                CREATE TABLE SavedFilters (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    UserId BIGINT NOT NULL,
                    Name NVARCHAR(50) NOT NULL,
                    JSON NVARCHAR(MAX) NOT NULL,
                    CONSTRAINT FK_SavedFilters_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
            """);

            Console.WriteLine("Все таблицы созданы успешно.");
        }

        private async Task CreateTableIfNotExistsAsync(SqlConnection conn, string tableName, string Sql)
        {
            if (!Regex.IsMatch(tableName, @"^[A-Za-z_][A-Za-z0-9_]*$"))
                throw new PotentialSqlInjectionException(tableName);

            using var IsTableExistsCommand = new SqlCommand(
                $"""
                IF OBJECT_ID('{tableName}', 'U') IS NULL
                    SELECT 0
                ELSE
                    SELECT 1
                """, 
                conn
            );

            object? result = await IsTableExistsCommand.ExecuteScalarAsync();
            bool exists = Convert.ToInt32(result) == 1;

            if (!exists)
            {
                using var createTableCmd = new SqlCommand(Sql, conn);
                await createTableCmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Таблица '{tableName}' создана.");
            }
            else
            {
                Console.WriteLine($"Таблица '{tableName}' уже существует, пропускаем создание.");
            }
        }
    }
}
