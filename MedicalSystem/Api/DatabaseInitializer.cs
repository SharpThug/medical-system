using Microsoft.Data.SqlClient;

namespace Api
{
    public class DatabaseInitializer
    {
        private readonly IDatabaseService _dbService;
        private readonly ITableCreationService _tableCreator;
        private readonly IDatabaseSeeder _seedService;
        private readonly IDatabasePrinter _printer;

        public DatabaseInitializer(IDatabaseService dbService, ITableCreationService tableCreator, IDatabaseSeeder seedService, IDatabasePrinter printer)
        {
            _dbService = dbService;
            _tableCreator = tableCreator;
            _seedService = seedService;
            _printer = printer;
        }

        public async Task InitAsync(string dbName, bool dropDbOnStart)
        {
            Console.WriteLine("Начало инициализации базы данных");

            if (dropDbOnStart)
            {
                await _dbService.DropDatabaseAsync(dbName);
            }

            if (!await _dbService.DatabaseExistsAsync(dbName))
            {
                await _dbService.CreateDatabaseAsync(dbName);
            }

            await _tableCreator.CreateAllTablesAsync();
            await _seedService.SeedDataAsync();
            await _printer.PrintAsync();

            Console.WriteLine("База данных успешно проинциализирована");
        }
    }
}
