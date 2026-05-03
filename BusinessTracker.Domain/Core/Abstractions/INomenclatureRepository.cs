using BusinessTracker.Domain.Models;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий номенклатуры.
/// </summary>
public interface INomenclatureRepository
{
    /// <summary>
    ///     Сохранить позиции номенклатуры в БД.
    /// </summary>
    Task SaveAsync(IEnumerable<Nomenclature> nomenclatures, CancellationToken token);

    /// <summary>
    ///     Получить всю номенклатуру, принадлежащую указанной категории.
    /// </summary>
    Task<IEnumerable<Nomenclature>> GetByCategoryAsync(Guid categoryId, CancellationToken token);
}
