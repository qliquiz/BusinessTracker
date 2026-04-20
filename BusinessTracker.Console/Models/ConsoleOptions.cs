namespace BusinessTracker.Console.Models;

/// <summary>
///     Настройки консольного приложения.
/// </summary>
public class ConsoleOptions
{
    /// <summary>
    ///     Строка подключения к MS SQL (legacy журнал).
    /// </summary>
    public string MsSqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     Базовый URL API для отправки транзакций.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "http://localhost:8000";

    /// <summary>
    ///     Идентификатор филиала (Branch), данные которого загружает этот экземпляр консоли.
    /// </summary>
    public Guid CompanyId { get; set; }
}