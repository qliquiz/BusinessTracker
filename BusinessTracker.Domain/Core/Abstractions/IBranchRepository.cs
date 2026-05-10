using BusinessTracker.Domain.Models;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий для получения списка филиалов.
/// </summary>
public interface IBranchRepository
{
    /// <summary>
    ///     Получить все филиалы со связанными организациями.
    /// </summary>
    Task<IEnumerable<Branch>> GetAllAsync(CancellationToken cancellationToken);
}