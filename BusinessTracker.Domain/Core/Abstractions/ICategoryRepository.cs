using BusinessTracker.Domain.Models;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий категорий номенклатуры.
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    ///     Сохранить категории в БД.
    /// </summary>
    Task SaveAsync(IEnumerable<Category> categories, CancellationToken token);

    /// <summary>
    ///     Получить все категории организации.
    /// </summary>
    Task<IEnumerable<Category>> GetByOwnerAsync(Guid organizationId, CancellationToken token);
}