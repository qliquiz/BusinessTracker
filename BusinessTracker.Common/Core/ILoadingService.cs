using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Common.Core;

/// <summary>
/// Сервис загрузки транзакций от клиентского приложения.
/// </summary>
public interface ILoadingService
{
    /// <summary>
    /// Записать транзакции синхронно.
    /// </summary>
    public bool Push(Organization organization, IEnumerable<JournalRowDto> transactions, CancellationToken token);

    /// <summary>
    /// Записать транзакции асинхронно.
    /// </summary>
    public Task<bool> PushAsync(Organization organization, IEnumerable<JournalRowDto> transactions, CancellationToken token);
}
