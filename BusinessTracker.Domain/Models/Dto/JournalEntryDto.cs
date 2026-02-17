using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models.Dto;

/// <summary>
/// DTO записи в журнале.
/// </summary>
public class JournalEntryDto
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    [Required]
    public long Id { get; set; }

    /// <summary>
    /// Уникальный номер чека.
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string CheckNumber { get; set; } = string.Empty;

    /// <summary>
    /// Уникальный идентификатор сотрудника.
    /// </summary>
    public long? EmployeeId { get; set; }

    /// <summary>
    /// Уникальный идентификатор номенклатуры.
    /// </summary>
    public long? NomenclatureId { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    [StringLength(255)]
    public string? Description { get; set; }

    /// <summary>
    /// Уникальный идентификатор категории.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Уникальный идентификатор операции.
    /// </summary>
    [Required]
    public long OperationId { get; set; }

    /// <summary>
    /// Дата транзакции с временной зоной.
    /// </summary>
    public DateTimeOffset TransactionDate { get; set; }

    /// <summary>
    /// Количество.
    /// </summary>
    [Required]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Скидка должна быть положительной")]
    public decimal DiscountAmount { get; set; }
}