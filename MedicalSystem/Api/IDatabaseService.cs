namespace Api
{
    public interface IDatabaseService
    {
        Task<bool> DatabaseExistsAsync(string dbName);
        Task CreateDatabaseAsync(string dbName);
        Task DropDatabaseAsync(string dbName);
    }
}
