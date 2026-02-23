using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Attributes;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель организации.
/// </summary>
public class Organization : DomainModel
{
    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// ИНН.
    /// </summary>
    [Required]
    [Template(@"^[0-9]{10}$")]
    public string Inn { get; set; } = string.Empty;

    /// <summary>
    /// Юридический адрес (формат КЛАДР).
    /// </summary>
    [Required]
    [Template(@"^(?=.*,.*,.*,.*,.*,.*)(?=.*,.*,.*,.*,.*,.*)?[\s\S]*$")]
    public required string Address { get; set; } = string.Empty;
}