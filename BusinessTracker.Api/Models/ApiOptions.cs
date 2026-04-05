namespace BusinessTracker.Api.Models;

/// <summary>
///     Настройки API-приложения.
/// </summary>
public class ApiOptions
{
    /// <summary>
    ///     Строка подключения к PostgresSQL.
    /// </summary>
    public string PostgresConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     Строка подключения к MSSQL (если требуется прямое чтение из legacy-БД).
    /// </summary>
    public string MsSqlConnectionString { get; set; } = string.Empty;
}