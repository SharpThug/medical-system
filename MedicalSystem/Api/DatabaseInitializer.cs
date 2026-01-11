using Microsoft.Data.SqlClient;

namespace Api
{
    public class DatabaseInitializer
    {
        public async Task Initialize(WebApplication app)
        {
            var config = app.Configuration;
            var connectionString = config.GetConnectionString("DefaultConnection");

            Console.WriteLine("Проверка существования базы данных...");

            var masterBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };

            using (var masterConn = new SqlConnection(masterBuilder.ConnectionString))
            {
                await masterConn.OpenAsync();

                var checkDbCommand = new SqlCommand("SELECT db_id('Db')", masterConn);
                var exists = await checkDbCommand.ExecuteScalarAsync();

                if (exists != DBNull.Value && exists != null)
                {
                    Console.WriteLine("База данных уже существует.");
                }
                else
                {
                    Console.WriteLine("База данных не найдена. Создаём её...");
                    var createDbScript = Path.Combine(app.Environment.ContentRootPath, "Scripts", "01_CreateDatabase.sql");
                    var script = await File.ReadAllTextAsync(createDbScript);

                    using var cmd = new SqlCommand(script, masterConn);
                    await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine("База данных создана успешно!");
                }
            }

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            bool departmentsExist = await CheckIfTableExists(conn, "Departments");
            bool usersExist = await CheckIfTableExists(conn, "Users");

            if (!departmentsExist || !usersExist)
            {
                Console.WriteLine("Создание таблиц и вставка данных...");

                var createTablesScript = Path.Combine(app.Environment.ContentRootPath, "Scripts", "02_CreateTables.sql");
                var script = await File.ReadAllTextAsync(createTablesScript);
                using (var cmd = new SqlCommand(script, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Таблицы созданы.");

                var seedDataScript = Path.Combine(app.Environment.ContentRootPath, "Scripts", "03_InsertSeedData.sql");
                script = await File.ReadAllTextAsync(seedDataScript);
                using (var cmd = new SqlCommand(script, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Данные вставлены.");
            }
            else
            {
                Console.WriteLine("Таблицы уже существуют. Пропускаем создание и вставку данных.");
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
            Console.WriteLine("\nДанные из Departments:");
            if (await CheckIfTableExists(conn, "Departments"))
            {
                var deptCmd = new SqlCommand("SELECT DepartmentID, DepartmentName, DepartmentCode, DepartmentType FROM Departments", conn);
                using var reader = await deptCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"ID: {reader["DepartmentID"]}, Name: {reader["DepartmentName"]}, Code: {reader["DepartmentCode"]}, Type: {reader["DepartmentType"]}");
                }
            }
            else
            {
                Console.WriteLine("Таблица Departments не найдена.");
            }

            Console.WriteLine("\nДанные из Users:");
            if (await CheckIfTableExists(conn, "Users"))
            {
                var userCmd = new SqlCommand("SELECT UserID, Login, FirstName, LastName, Role, DepartmentID FROM Users", conn);
                using var reader = await userCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"ID: {reader["UserID"]}, Login: {reader["Login"]}, Name: {reader["FirstName"]} {reader["LastName"]}, Role: {reader["Role"]}, DeptID: {reader["DepartmentID"]}");
                }
            }
            else
            {
                Console.WriteLine("Таблица Users не найдена.");
            }
        }
    }
}
