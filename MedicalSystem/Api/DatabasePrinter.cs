namespace Api
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    public class DatabasePrinter : IDatabasePrinter
    {
        private readonly string _connectionString;

        public DatabasePrinter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task PrintAsync()
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await PrintDepartments(conn);
            await PrintUsers(conn);
        }

        private async Task PrintDepartments(SqlConnection conn)
        {
            using SqlCommand cmd = new SqlCommand(
                """
                SELECT Id, Name, Code, Type 
                FROM Departments
                """,
                conn
            );
            using var reader = await cmd.ExecuteReaderAsync();

            Console.WriteLine("Departments:");
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Code: {reader["Code"]}, Type: {reader["Type"]}");
            }
        }

        private async Task PrintUsers(SqlConnection conn)
        {
            using SqlCommand cmd = new SqlCommand(
                """
                SELECT u.Id, u.Login, u.Password, u.LastName, u.FirstName, u.Patronymic, u.Role, u.Specialty, u.DepartmentId, u.IsActive,
                       d.Name AS DepartmentName
                FROM Users u
                LEFT JOIN Departments d ON u.DepartmentId = d.Id
                """,
                conn
            );

            using var reader = await cmd.ExecuteReaderAsync();

            Console.WriteLine("\nUsers:");
            while (await reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    object value = reader[i];
                    Console.Write($"{columnName}: {value}, ");
                }
                Console.WriteLine();
            }
        }
    }
}
