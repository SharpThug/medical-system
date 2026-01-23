using System.ComponentModel.DataAnnotations;

namespace Api
{
    public record LoginRequest
    {
        [Required(ErrorMessage = "Необходимо указать логин")]
        public string Login { get; init; } = null!;

        [Required(ErrorMessage = "Необходимо указать пароль")]
        public string Password { get; init; } = null!;
    }
}
