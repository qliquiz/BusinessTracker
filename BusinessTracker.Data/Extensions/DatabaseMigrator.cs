using DbUp;

namespace BusinessTracker.Data.Extensions;

/// <summary>
///     Применяет SQL-миграции через DbUp при старте приложения.
/// </summary>
public static class DatabaseMigrator
{
    /// <summary>
    ///     Выполняет все необработанные миграции из папки Scripts сборки BusinessTracker.Data.
    ///     Скрипты запускаются в алфавитном порядке (V001__, V002__, …).
    /// </summary>
    /// <exception cref="InvalidOperationException">Если одна из миграций завершилась с ошибкой.</exception>
    public static void Migrate(string connectionString)
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(BusinessTrackerContext).Assembly)
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
            throw new InvalidOperationException(
                $"Database migration failed on script '{result.ErrorScript?.Name}': {result.Error?.Message}",
                result.Error);
    }
}