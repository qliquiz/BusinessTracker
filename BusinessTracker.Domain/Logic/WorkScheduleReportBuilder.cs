using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Logic;

/// <summary>
/// Построитель отчёта "График работы" на основе доменных моделей транзакций.
/// Каждая строка — одна смена сотрудника (начало + конец).
/// Незакрытая смена имеет <see cref="WorkScheduleReportRowDto.ShiftEnd"/> == null.
/// </summary>
public static class WorkScheduleReportBuilder
{
    /// <summary>
    /// Сформировать отчёт по сменам сотрудников.
    /// Для каждого StartShift ищется ближайший StopShift того же сотрудника.
    /// </summary>
    public static IEnumerable<WorkScheduleReportRowDto> Build(IEnumerable<Transaction> transactions)
    {
        var shiftEvents = transactions
            .Where(t => t.Type is TransactionType.StartShift or TransactionType.StopShift)
            .OrderBy(t => t.TransactionDate)
            .ToList();

        var stops = shiftEvents
            .Where(t => t.Type == TransactionType.StopShift)
            .ToList();

        return shiftEvents
            .Where(t => t.Type == TransactionType.StartShift)
            .Select(start =>
            {
                var stop = stops.FirstOrDefault(s =>
                    s.Employee.Id == start.Employee.Id &&
                    s.TransactionDate > start.TransactionDate);

                return new WorkScheduleReportRowDto
                {
                    EmployeeId = start.Employee.Id,
                    EmployeeName = start.Employee.Name,
                    ShiftStart = start.TransactionDate,
                    ShiftEnd = stop?.TransactionDate,
                    OrganizationId = start.Owner.Id
                };
            });
    }
}
