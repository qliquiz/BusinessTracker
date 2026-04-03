using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий отчёта "Продажи".
/// </summary>
public interface ISalesReportRepository
{
    /// <summary>
    ///     Получить строки отчёта по организации за период.
    /// </summary>
    Task<IEnumerable<SalesReportRowDto>> GetReport(
        Guid organizationId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken);
}