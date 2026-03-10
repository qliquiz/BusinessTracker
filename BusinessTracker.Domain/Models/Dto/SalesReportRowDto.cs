using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models.Dto;

/// <summary>
/// Строка отчёта "Продажи".
/// </summary>
public class SalesReportRowDto : IDto
{
    /// <summary>
    /// Код группы (категории).
    /// </summary>
    public Guid CategoryId { get; init; }

    /// <summary>
    /// Название группы (категории).
    /// </summary>
    public string CategoryName { get; init; } = string.Empty;

    /// <summary>
    /// Код номенклатуры.
    /// </summary>
    public Guid NomenclatureId { get; init; }

    /// <summary>
    /// Название номенклатуры.
    /// </summary>
    public string NomenclatureName { get; init; } = string.Empty;

    /// <summary>
    /// Количество.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    public decimal DiscountAmount { get; init; }

    /// <summary>
    /// Код организации.
    /// </summary>
    public Guid OrganizationId { get; init; }
}