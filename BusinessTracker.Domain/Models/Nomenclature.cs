using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель номенклатуры.
/// </summary>
public class Nomenclature : BaseEntity
{
    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}