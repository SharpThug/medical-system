namespace Api
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}
