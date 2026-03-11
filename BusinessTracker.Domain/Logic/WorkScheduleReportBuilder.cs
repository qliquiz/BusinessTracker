using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Logic;

/// <summary>
/// Построитель отчёта "График работы" на основе доменных моделей транзакций.
/// <para>
/// Алгоритм: события сортируются по времени, затем по каждому сотруднику
/// StartShift ставится в очередь. Каждый StopShift закрывает первый открытый StartShift.
/// Незакрытые смены добавляются в конец с <see cref="WorkScheduleReportRowDto.ShiftEnd"/> = null.
/// Осиротевшие StopShift (без предшествующего StartShift) игнорируются.
/// </para>
/// </summary>
public static class WorkScheduleReportBuilder
{
    /// <summary>
    /// Сформировать отчёт по сменам сотрудников.
    /// </summary>
    public static IEnumerable<WorkScheduleReportRowDto> Build(IEnumerable<Transaction> transactions)
    {
        var result = new List<WorkScheduleReportRowDto>();

        var byEmployee = transactions
            .Where(t => t.Type is TransactionType.StartShift or TransactionType.StopShift)
            .GroupBy(t => t.Employee.Id);

        foreach (var group in byEmployee)
        {
            var events = group.OrderBy(t => t.TransactionDate).ToList();
            var employee = events[0].Employee;
            var org = events[0].Owner;

            // Открытые смены в порядке поступления
            var openShifts = new Queue<DateTimeOffset>();

            foreach (var ev in events)
            {
                if (ev.Type == TransactionType.StartShift)
                {
                    openShifts.Enqueue(ev.TransactionDate);
                }
                else // StopShift
                {
                    if (!openShifts.TryDequeue(out var shiftStart))
                        continue; // осиротевший StopShift — пропускаем

                    result.Add(new WorkScheduleReportRowDto
                    {
                        EmployeeId = employee.Id,
                        EmployeeName = employee.Name,
                        ShiftStart = shiftStart,
                        ShiftEnd = ev.TransactionDate,
                        OrganizationId = org.Id
                    });
                }
            }

            // Незакрытые смены
            foreach (var shiftStart in openShifts)
            {
                result.Add(new WorkScheduleReportRowDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.Name,
                    ShiftStart = shiftStart,
                    ShiftEnd = null,
                    OrganizationId = org.Id
                });
            }
        }

        return result;
    }
}