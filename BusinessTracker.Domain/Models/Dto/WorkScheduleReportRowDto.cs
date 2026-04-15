using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models.Dto;

/// <summary>
///     Строка отчёта "График работы".
/// </summary>
public class WorkScheduleReportRowDto : IDto
{
    /// <summary>
    ///     Код сотрудника.
    /// </summary>
    public Guid EmployeeId { get; init; }

    /// <summary>
    ///     ФИО сотрудника.
    /// </summary>
    public string EmployeeName { get; init; } = string.Empty;

    /// <summary>
    ///     Дата и время начала смены.
    /// </summary>
    public DateTimeOffset ShiftStart { get; init; }

    /// <summary>
    ///     Дата и время окончания смены (null — смена ещё не закрыта).
    /// </summary>
    public DateTimeOffset? ShiftEnd { get; init; }

    /// <summary>
    ///     Код организации.
    /// </summary>
    public Guid OrganizationId { get; init; }
}