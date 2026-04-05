using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий для записи строк журнала в плоскую таблицу БД.
/// </summary>
public interface IJournalRowsRepository
{
    /// <summary>
    ///     Сохранить строки журнала для указанной организации.
    /// </summary>
    Task SaveAsync(Guid organizationId, IEnumerable<JournalRowDto> transactions, CancellationToken token);
}