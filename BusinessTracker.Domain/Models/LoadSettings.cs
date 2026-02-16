using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель настроек загрузки.
/// </summary>
public class LoadSettings
{
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