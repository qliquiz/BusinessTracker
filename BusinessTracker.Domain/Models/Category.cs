using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель категории номераклатуры.
/// </summary>
public class Category
{
    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;
}