using BusinessTracker.Data;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    try
    {
        Console.WriteLine("Development environment detected. Initializing database...");
        var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");
        }

        var initializer = new DatabaseInitializer(connectionString);
        await initializer.ResetDatabaseAsync();
        Console.WriteLine("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred during database initialization: {ex.Message}");
    }
}

app.Run();