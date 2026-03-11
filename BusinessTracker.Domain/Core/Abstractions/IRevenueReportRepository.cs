using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
/// Репозиторий отчёта "Выручка".
/// </summary>
public interface IRevenueReportRepository
{
    /// <summary>
    /// Получить строки отчёта по организации за период.
    /// </summary>
    Task<IEnumerable<RevenueReportRowDto>> GetReport(
        Guid organizationId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken);
}
