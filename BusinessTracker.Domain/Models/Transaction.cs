using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель транзакции.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Сумма.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Amount { get; set; }
    
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
    
    public required Organization Organization { get; set; }
    public required Nomenclature Nomenclature { get; set; }
    public required Employee Employee { get; set; }
}