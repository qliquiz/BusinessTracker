using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Api.Models;

/// <summary>
/// Запрос на загрузку транзакций от клиентского приложения.
/// </summary>
public class PushTransactionsRequest
{
    /// <summary>
    /// Идентификатор организации.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Список транзакций из журнала.
    /// </summary>
    public IEnumerable<JournalRowDto> Transactions { get; set; } = [];
}