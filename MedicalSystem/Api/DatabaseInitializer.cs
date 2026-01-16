using Microsoft.Data.SqlClient;

namespace Api
{
    public class DatabaseInitializer
    {
        public async Task Initialize(WebApplication app)
        {
            IConfiguration config = app.Configuration;
            string masterConnectionString = config.GetConnectionString("MasterConnection")!;
            string defaultConnectionString = config.GetConnectionString("DefaultConnection")!;
            string dbName = config.GetValue<string>("DatabaseSettings:DatabaseName")!;
            bool dropDbOnStart = config.GetValue<bool>("DatabaseSettings:DropDatabaseOnStart");

            Console.WriteLine("Проверка существования базы данных..."); //TODO: в лог полетит

            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                await masterConnection.OpenAsync();

                if (dropDbOnStart)
                {
                    await DropDatabase(masterConnection, dbName);
                }

                SqlCommand checkDbCommand = new SqlCommand(
                    "SELECT DB_ID(@dbName)",
                    masterConnection
                );

                checkDbCommand.Parameters.AddWithValue("@dbName", dbName);
                var exists = await checkDbCommand.ExecuteScalarAsync();

                if (exists != DBNull.Value && exists != null)
                {
                    Console.WriteLine("База данных уже существует.");  //TODO: в лог полетит
                }
                else
                {
                    string createDbSql = $"CREATE DATABASE [{dbName}]";
                    using var createCmd = new SqlCommand(createDbSql, masterConnection);
                    await createCmd.ExecuteNonQueryAsync();

                    Console.WriteLine("База данных создана успешно!");   //TODO: в лог полетит
                }
            }

            using var conn = new SqlConnection(defaultConnectionString);
            await conn.OpenAsync();

            bool departmentsExist = await CheckIfTableExists(conn, "Departments");
            bool usersExist = await CheckIfTableExists(conn, "Users");

            if (!departmentsExist || !usersExist)
            {
                Console.WriteLine("Создание таблиц и вставка данных...");   //TODO: в лог полетит


                string createAllTables = @"
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

                CREATE TABLE Departments (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(50) UNIQUE NOT NULL,
                    Code NVARCHAR(20) UNIQUE NOT NULL,
                    Type NVARCHAR(50) NOT NULL,
                    HeadDoctorId INT,
                    IsActive BIT DEFAULT 1
                );

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

                CREATE TABLE Diagnoses (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Code NVARCHAR(10) NOT NULL,
                    Name NVARCHAR(50) NOT NULL,
                    Category NVARCHAR(50) NOT NULL,
                    Description NVARCHAR(500)
                );

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

                CREATE TABLE SavedFilters (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    UserId BIGINT NOT NULL,
                    Name NVARCHAR(50) NOT NULL,
                    JSON NVARCHAR(MAX) NOT NULL,
                    CONSTRAINT FK_SavedFilters_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
                ";

                using (var cmd = new SqlCommand(createAllTables, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Таблицы созданы.");    //TODO: в лог полетит





                Console.WriteLine("Заполнение начальными данными...");      //TODO: в лог полетит

                var departments = new[]
                {
                    new { Name = "Офтальмология", Code = "OFT", Type = "Хирургическое" },
                    new { Name = "Терапия", Code = "TER", Type = "Терапевтическое" }
                };

                foreach (var dept in departments)
                {
                    string insertSql = @"
                    IF NOT EXISTS (SELECT 1 FROM Departments WHERE Code = @Code)
                    BEGIN
                        INSERT INTO Departments (Name, Code, Type)
                        VALUES (@Name, @Code, @Type)
                    END";

                    using var cmd = new SqlCommand(insertSql, conn);
                    cmd.Parameters.AddWithValue("@Name", dept.Name);
                    cmd.Parameters.AddWithValue("@Code", dept.Code);
                    cmd.Parameters.AddWithValue("@Type", dept.Type);

                    await cmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Начальные данные успешно вставлены!");     //TODO: в лог полетит
            }
            else
            {
                Console.WriteLine("Таблицы уже существуют. Пропускаем создание и вставку данных.");     //TODO: в лог полетит
            }

            await PrintData(conn);
        }

        private async Task<bool> CheckIfTableExists(SqlConnection conn, string tableName)
        {
            var cmd = new SqlCommand($"IF OBJECT_ID('{tableName}', 'U') IS NOT NULL SELECT 1 ELSE SELECT 0", conn);
            var result = await cmd.ExecuteScalarAsync();
            return result != null && (int)result == 1;
        }

        private async Task PrintData(SqlConnection conn)
        {
            Console.WriteLine("\nДанные из Departments:");                  //TODO: в лог полетит
            if (await CheckIfTableExists(conn, "Departments"))
            {
                var deptCmd = new SqlCommand("SELECT Id, Name, Code, Type FROM Departments", conn);
                using var reader = await deptCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Code: {reader["Code"]}, Type: {reader["Type"]}");        //TODO: в лог полетит
                }
            }
            else
            {
                Console.WriteLine("Таблица Departments не найдена.");         //TODO: в лог полетит
            }

            Console.WriteLine("\nДанные из Users:");                    //TODO: в лог полетит
            if (await CheckIfTableExists(conn, "Users"))
            {
                var userCmd = new SqlCommand("SELECT Id, Login, LastName, FirstName, Role, DepartmentId FROM Users", conn);
                using var reader = await userCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"ID: {reader["UserID"]}, Login: {reader["Login"]}, Name: {reader["FirstName"]} {reader["LastName"]}, Role: {reader["Role"]}, DeptID: {reader["DepartmentID"]}");    //TODO: в лог полетит
                }
            }
            else
            {
                Console.WriteLine("Таблица Users не найдена.");       //TODO: в лог полетит
            }
        }

        private async Task DropDatabase(SqlConnection masterConn, string dbName)
        {
            string dropScript = $@"
                USE master;

                IF DB_ID('{dbName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{dbName}];
                END";

            using (var cmd = new SqlCommand(dropScript, masterConn))
            {
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"База данных '{dbName}' удалена.");    //TODO: в лог полетит
            }
        }
    }
}
