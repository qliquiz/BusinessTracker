namespace BusinessTracker.Console.Models;

/// <summary>
/// Настройки приложения.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Строка подключения MS SQL
    /// </summary>
    public required string ConnectionString { get; set; } = string.Empty;
}