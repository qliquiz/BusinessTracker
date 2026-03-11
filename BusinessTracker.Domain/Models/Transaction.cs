using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Enums;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель транзакции.
/// </summary>
public class Transaction : DomainModel
{
    /// <summary>
    /// Тип транзакции.
    /// </summary>
    [Required]
    public required TransactionType Type { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public required decimal Discount { get; set; }

    /// <summary>
    /// Количество.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Дата и время.
    /// </summary>
    [Required]
    public required DateTimeOffset TransactionDate { get; set; }

    /// <summary>
    /// Организация владелец.
    /// </summary>
    [Required]
    public required Organization Owner { get; set; } = null!;

    /// <summary>
    /// Номенклатура.
    /// </summary>
    [Required]
    public required Nomenclature Nomenclature { get; set; } = null!;

    /// <summary>
    /// Сотрудник, который выполнил операцию.
    /// </summary>
    [Required]
    public required Employee Employee { get; set; } = null!;

}