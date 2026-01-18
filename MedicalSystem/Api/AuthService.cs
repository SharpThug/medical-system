using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api
{
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;
        private readonly string _jwtKey;

        public AuthService(string connectionString, string jwtKey)
        {
            _connectionString = connectionString;
            _jwtKey = jwtKey;
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            User? user = await GetUserByLoginAsync(login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid login or password"); //другое исключение надо кидать
            }

            return GenerateJwtToken(user);
        }

        private async Task<User> GetUserByLoginAsync(string login)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(
                """
            SELECT * 
            FROM Users 
            WHERE Login=@login AND IsActive=1
            """,
                conn
            );
            cmd.Parameters.AddWithValue("@login", login);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new UnauthorizedAccessException("Invalid login or password"); //другое исключение надо кидать
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

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_jwtKey);
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(12),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
