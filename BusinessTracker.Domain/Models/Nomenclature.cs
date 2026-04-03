using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
///     Модель номенклатуры.
/// </summary>
public class Nomenclature : DomainModel
{
    /// <summary>
    ///     Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Категория.
    /// </summary>
    [Required]
    public required Category Category { get; set; } = null!;
}