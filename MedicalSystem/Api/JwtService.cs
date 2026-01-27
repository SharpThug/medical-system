using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Api
{
    public class JwtService : IJwtService
    {
        private readonly string _jwtKey;

        public JwtService(string jwtKey)
        {
            _jwtKey = jwtKey;
        }

        public string GenerateJwtToken(User user)
        {
            byte[] key = Encoding.UTF8.GetBytes(_jwtKey);
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

            JwtSecurityToken token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(12),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
