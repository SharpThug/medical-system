using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Shared;

namespace Api
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            Console.WriteLine("Попытка авторизации");//логин и пароль тут выведешь еще

            User user = await _userRepository.GetByLoginAsync(login);

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new InvalidCredentialsException("Неверный логин или пароль");
            }

            Console.WriteLine("Успешная авторизация, сейчас будет сгенерирован JWT-токен");

            return _jwtService.GenerateJwtToken(user);
        }
    }
}
