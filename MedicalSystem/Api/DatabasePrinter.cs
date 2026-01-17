namespace Api
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    public class DatabasePrinter : IDataPrinter
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
    }
}
