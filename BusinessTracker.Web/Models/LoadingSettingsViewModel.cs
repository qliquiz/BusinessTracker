using System.ComponentModel.DataAnnotations;

namespace BusinessTracker.Web.Models;

/// <summary>
///     View-модель формы редактирования настроек загрузки.
/// </summary>
public class LoadingSettingsViewModel
{
    [Required(ErrorMessage = "Выберите филиал")]
    public Guid BranchId { get; set; }

    [Required(ErrorMessage = "Введите описание")]
    [StringLength(255, ErrorMessage = "Максимальная длина — 255 символов")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите начальную позицию")]
    [Range(0, long.MaxValue, ErrorMessage = "Значение должно быть ≥ 0")]
    [Display(Name = "Начальная позиция")]
    public long StartPosition { get; set; }

    [Required(ErrorMessage = "Укажите размер пакета")]
    [Range(1, long.MaxValue, ErrorMessage = "Значение должно быть ≥ 1")]
    [Display(Name = "Размер пакета")]
    public long BatchSize { get; set; } = 1000;
}