using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core;
using BusinessTracker.Domain.Core.Attributes;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель организации.
/// </summary>
public class Organization : BaseEntity
{
    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// Юридический адрес (формат КЛАДР).
    /// </summary>
    [Required]
    [Address]
    public required string Address { get; set; } = string.Empty;
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}