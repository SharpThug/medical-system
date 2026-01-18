using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SqlConnection _conn;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _conn = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await GetUserByLoginAsync(request.Login);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        private async Task<User?> GetUserByLoginAsync(string login)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            using var cmd = new SqlCommand(
                "SELECT TOP 1 * FROM Users WHERE Login=@login AND IsActive=1",
                conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read()) return null;

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
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

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
