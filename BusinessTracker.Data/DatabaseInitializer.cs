using Npgsql;

namespace BusinessTracker.Data;

public class DatabaseInitializer(string connectionString)
{
    public async Task ResetDatabaseAsync()
    {
        var baseDir = AppContext.BaseDirectory;

        Console.WriteLine("Starting database reset...");
        await ExecuteSqlFromFileAsync(Path.Combine(baseDir, "SqlScripts", "create_tables.sql"));
        await ExecuteSqlFromFileAsync(Path.Combine(baseDir, "SqlScripts", "seed_data.sql"));
        Console.WriteLine("Database reset completed successfully.");
    }

    private async Task ExecuteSqlFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: SQL script file not found at '{filePath}'");
            throw new FileNotFoundException("SQL script file not found.", filePath);
        }

        var scriptContent = await File.ReadAllTextAsync(filePath);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(scriptContent, connection);
        try
        {
            await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Successfully executed script: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing script {filePath}: {ex.Message}");
            throw;
        }
    }
}