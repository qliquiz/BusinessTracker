using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
///     Модель филиала организации.
/// </summary>
public class Branch : DomainModel
{
    /// <summary>
    ///     Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    /// <summary>
    ///     Организация-владелец.
    /// </summary>
    [Required]
    public Organization Owner { get; set; } = null!;
}