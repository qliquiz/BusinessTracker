using BusinessTracker.Domain.Models;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий сотрудников.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    ///     Сохранить сотрудников в БД.
    /// </summary>
    Task SaveAsync(IEnumerable<Employee> employees, CancellationToken token);

    /// <summary>
    ///     Получить всех сотрудников организации.
    /// </summary>
    Task<IEnumerable<Employee>> GetByOwnerAsync(Guid organizationId, CancellationToken token);
}