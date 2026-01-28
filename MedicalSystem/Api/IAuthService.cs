namespace Api
{
    public interface IAuthService
    {
        public Task<string> LoginAsync(string login, string password);
    }
}
