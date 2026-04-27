using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Детектор новых категорий: сравнивает категории из строк журнала с уже
///     существующими в БД и возвращает только те, которые нужно создать.
/// </summary>
public interface INewCategoriesDetector
{
    /// <summary>
    ///     Определить категории из набора строк журнала, отсутствующие в БД для указанной организации.
    /// </summary>
    Task<IEnumerable<Category>> DetectAsync(Organization owner, IEnumerable<JournalRowDto> rows,
        CancellationToken token);
}
