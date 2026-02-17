using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель номенклатуры.
/// </summary>
public class Nomenclature : IId
{
    public Guid Id { get; init; }

    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;
    
    public required Category Category { get; set; }
}