using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Источник необработанных строк журнала из плоской таблицы <c>JournalRows</c>.
/// </summary>
public interface IJournalDataSource
{
    /// <summary>
    ///     Возвращает строки журнала, у которых <c>Code &gt;= startPosition</c>,
    ///     отсортированные по возрастанию кода и ограниченные <paramref name="batchSize" />.
    /// </summary>
    Task<IEnumerable<JournalRowDto>> GetUnprocessedRowsAsync(Guid organizationId, long startPosition, int batchSize,
        CancellationToken token);
}