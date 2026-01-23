namespace Api
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginAsync(string login);
    }
}
