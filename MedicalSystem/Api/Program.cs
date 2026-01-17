using Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var dbInitializer = new DatabaseInitializer();
await dbInitializer.Init(app);
// ----------------------------------

app.Run();