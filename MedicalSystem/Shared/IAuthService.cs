namespace Shared
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string login, string password);
    }
}
