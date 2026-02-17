using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель настроек загрузки.
/// </summary>
public class LoadSettings : IId
{
    public Guid Id { get; init; }

    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string Name { get; set; } = "Default";

    /// <summary>
    /// Период хранения данных в месяцах.
    /// </summary>
    [Range(1, 36)]
    public int DataRetentionMonths { get; set; } = 3;
}