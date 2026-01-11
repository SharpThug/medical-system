using Microsoft.Data.SqlClient;

namespace Api
{
    public class DatabaseInitializer
    {
        public async Task Initialize(WebApplication app)
        {
            var config = app.Configuration;
            var defaultConnectionString = config.GetConnectionString("DefaultConnection");

            var masterBuilder = new SqlConnectionStringBuilder(defaultConnectionString)
            {
                InitialCatalog = "master"
            };

            var dbBuilder = new SqlConnectionStringBuilder(defaultConnectionString)
            {
                InitialCatalog = "Db"
            };

            Console.WriteLine("Проверка существования базы данных...");

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

                    // Скрипт создания базы
                    var createDbScript = Path.Combine(app.Environment.ContentRootPath, "Scripts", "01_CreateDatabase.sql");
                    var script = await File.ReadAllTextAsync(createDbScript);

                    using var cmd = new SqlCommand(script, masterConn);
                    await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine("База данных создана успешно!");
                }
            }

            // 2️⃣ Работаем с рабочей базой Db для таблиц и данных
            using var conn = new SqlConnection(dbBuilder.ConnectionString);
            await conn.OpenAsync();

            bool departmentsExist = await CheckIfTableExists(conn, "Departments");
            bool usersExist = await CheckIfTableExists(conn, "Users");

            if (!departmentsExist || !usersExist)
            {
                Console.WriteLine("Создание таблиц и вставка данных...");

                // Скрипт создания таблиц
                var createTablesScript = Path.Combine(app.Environment.ContentRootPath, "Scripts", "02_CreateTables.sql");
                var script = await File.ReadAllTextAsync(createTablesScript);
                using (var cmd = new SqlCommand(script, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Таблицы созданы.");

                // Скрипт seed данных
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

            // 3️⃣ Вывод данных в консоль
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
