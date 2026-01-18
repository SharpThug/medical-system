using Microsoft.Data.SqlClient;

namespace Api
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(       //TODO:надо проверять статус active (это в самом конце сделаешь)
                """
                SELECT * 
                FROM Users 
                WHERE Login = @login AND IsActive = 1
                """,
                conn
            );
            cmd.Parameters.AddWithValue("@login", login);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new UserNotFoundException("Пользователь с таким логином не найден");   //бизнесовое надо бросать из сервиса
            }

            return new User
            {
                Id = (long)reader["Id"],
                Login = (string)reader["Login"],
                Password = (string)reader["Password"],
                Role = (string)reader["Role"],
                DepartmentId = (int)reader["DepartmentId"]
            };
        }
    }
}
