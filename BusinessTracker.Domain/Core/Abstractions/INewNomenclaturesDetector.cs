using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Детектор новой номенклатуры: сравнивает позиции из строк журнала с уже
///     существующими в БД и возвращает только те, которые нужно создать.
/// </summary>
public interface INewNomenclaturesDetector
{
    /// <summary>
    ///     Определить номенклатуру из набора строк журнала, отсутствующую в БД.
    ///     Принимает актуальный список категорий (включая только что сохранённые) для привязки.
    /// </summary>
    Task<IEnumerable<Nomenclature>> DetectAsync(IEnumerable<Category> knownCategories,
        IEnumerable<JournalRowDto> rows, CancellationToken token);
}
