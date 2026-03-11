using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models.Dto;

/// <summary>
/// Строка отчёта "Выручка".
/// </summary>
public class RevenueReportRowDto : IDto
{
    /// <summary>
    /// Период (дата).
    /// </summary>
    public DateOnly Period { get; init; }

    /// <summary>
    /// Сумма оплаты наличными.
    /// </summary>
    public decimal CashAmount { get; init; }

    /// <summary>
    /// Сумма оплаты безналично.
    /// </summary>
    public decimal NonCashAmount { get; init; }

    /// <summary>
    /// Сумма оплаты прочее.
    /// </summary>
    public decimal OtherAmount { get; init; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    public decimal DiscountAmount { get; init; }

    /// <summary>
    /// Флаг праздничного / выходного дня.
    /// </summary>
    public bool IsHoliday { get; init; }

    /// <summary>
    /// Код организации.
    /// </summary>
    public Guid OrganizationId { get; init; }
}