using Microsoft.Data.SqlClient;

namespace Api
{
    public class DatabaseCreationService : IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseCreationService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> DatabaseExistsAsync(string dbName)
        {
            await using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand("SELECT DB_ID(@dbName)", conn);
            cmd.Parameters.AddWithValue("@dbName", dbName);
            object? result = await cmd.ExecuteScalarAsync();

            return result != DBNull.Value;
        }

        public async Task CreateDatabaseAsync(string dbName)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = $"CREATE DATABASE [{dbName}]";
            using var cmd = new SqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"Базы данных '{dbName}' создана.");
        }

        public async Task DropDatabaseAsync(string dbName)
        {
            await using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand(
                $"""
                USE master;
                IF DB_ID('{dbName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{dbName}];
                END
                """,
                conn
            );
            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"Database '{dbName}' dropped.");
        }
    }
}
