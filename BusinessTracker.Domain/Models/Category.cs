using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель категории номераклатуры.
/// </summary>
public class Category : IId
{
    public Guid Id { get; init; }

    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;
    
    public virtual ICollection<Nomenclature> Nomenclatures { get; set; } = new List<Nomenclature>();
}