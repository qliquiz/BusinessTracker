using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Детектор новых сотрудников: сравнивает сотрудников из строк журнала с уже
///     существующими в БД и возвращает только тех, кого нужно создать.
/// </summary>
public interface INewEmployeesDetector
{
    /// <summary>
    ///     Определить сотрудников из набора строк журнала, отсутствующих в БД для указанной организации.
    /// </summary>
    Task<IEnumerable<Employee>> DetectAsync(Organization owner, IEnumerable<JournalRowDto> rows,
        CancellationToken token);
}
