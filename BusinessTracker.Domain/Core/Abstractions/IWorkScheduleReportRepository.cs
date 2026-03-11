using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
/// Репозиторий отчёта "График работы".
/// </summary>
public interface IWorkScheduleReportRepository
{
    /// <summary>
    /// Получить строки отчёта по организации за период.
    /// </summary>
    Task<IEnumerable<WorkScheduleReportRowDto>> GetReport(
        Guid organizationId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken);
}
