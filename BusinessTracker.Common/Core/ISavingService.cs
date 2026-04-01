using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Common.Core;

/// <summary>
/// Интерфейс для записи транзакций в плоскую таблицу БД.
/// </summary>
public interface ISavingService
{
    /// <summary>
    /// Сохранить транзакции синхронно.
    /// </summary>
    public bool Save(IEnumerable<JournalRowDto> transactions);

    /// <summary>
    /// Сохранить транзакции асинхронно.
    /// </summary>
    public Task<bool> SaveAsync(IEnumerable<JournalRowDto> transactions, CancellationToken token);
}
