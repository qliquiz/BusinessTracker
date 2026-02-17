using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Core.Attributes;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель организации.
/// </summary>
public class Organization : IId
{
    public Guid Id { get; init; }

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
}