using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Поставщик необработанных строк журнала на основе настроек загрузки.
///     Возвращает записи из <c>JournalRows</c>, ещё не прошедшие нормализацию
///     (Code &gt;= StartPosition, не более BatchSize записей).
/// </summary>
public interface INewJournalRowsProvider
{
    /// <summary>
    ///     Получить необработанные строки журнала в соответствии с настройками загрузки.
    /// </summary>
    Task<IEnumerable<JournalRowDto>> GetUnprocessedAsync(LoadingSettings settings, CancellationToken token);
}