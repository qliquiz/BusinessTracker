namespace BusinessTracker.Console.Models;

/// <summary>
/// Настройки консольного приложения.
/// </summary>
public class ConsoleOptions
{
    /// <summary>
    /// Строка подключения к MS SQL (legacy журнал).
    /// </summary>
    public string MsSqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Строка подключения к PostgreSQL.
    /// </summary>
    public string PostgreConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Базовый URL API для отправки транзакций.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "http://localhost:8000";
}
