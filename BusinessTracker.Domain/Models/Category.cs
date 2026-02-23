using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель категории номераклатуры.
/// </summary>
public class Category : DomainModel
{
    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// Организация владелец категории.
    /// </summary>
    [Required]
    public Organization Owner { get; set; } = null!;
}