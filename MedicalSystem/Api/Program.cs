using Api;

var builder = WebApplication.CreateBuilder(args);

string masterConnectionString = builder.Configuration.GetConnectionString("MasterConnection")!;
string defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
string jwtKey = builder.Configuration.GetValue<string>("Jwt:Key")!;
string dbName = builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName")!;
bool dropDbOnStart = builder.Configuration.GetValue<bool>("DatabaseSettings:DropDatabaseOnStart");

builder.Services.AddSingleton<IDatabaseService>(sp => new DatabaseCreationService(masterConnectionString));
builder.Services.AddSingleton<ITableCreationService>(sp => new TableCreationService(defaultConnectionString));
builder.Services.AddSingleton<IDatabaseSeeder>(sp => new DatabaseSeedingService(defaultConnectionString));
builder.Services.AddSingleton<IDatabasePrinter>(sp => new DatabasePrinter(defaultConnectionString));
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddSingleton<IUserRepository>(sp => new UserRepository(defaultConnectionString));

builder.Services.AddSingleton<IJwtService>(sp => new JwtService(jwtKey));

builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

DatabaseInitializer initializer = app.Services.GetRequiredService<DatabaseInitializer>();
await initializer.InitAsync(dbName, dropDbOnStart);

app.Run();