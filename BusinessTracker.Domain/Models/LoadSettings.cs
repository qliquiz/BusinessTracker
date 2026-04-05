using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Domain.Models;

/// <summary>
///     Модель настроек загрузки данных.
/// </summary>
public class LoadingSettings : DomainModel
{
    /// <summary>
    ///     Организация владелец.
    /// </summary>
    [Required]
    public Organization Owner { get; set; } = null!;

    /// <summary>
    ///     Описание.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Уникальный код транзакции для начала загрузки.
    /// </summary>
    [Required]
    public long StartPosition { get; set; }

    /// <summary>
    ///     Размер пакета данных для одной загрузки (количество записей).
    /// </summary>
    [Required]
    public long BatchSize { get; set; } = 1000;
}