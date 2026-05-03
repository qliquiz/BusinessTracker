using BusinessTracker.Domain.Models;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Репозиторий для финальной записи нормализованных бизнес-данных и чтения справочников.
/// </summary>
public interface IBusinessDataRepository
{
    /// <summary>Массово сохранить новые категории номенклатуры.</summary>
    Task SaveCategoriesAsync(IEnumerable<Category> categories, CancellationToken token);

    /// <summary>Массово сохранить новые позиции номенклатуры.</summary>
    Task SaveNomenclatureAsync(IEnumerable<Nomenclature> nomenclatures, CancellationToken token);

    /// <summary>Массово сохранить новых сотрудников.</summary>
    Task SaveEmployeesAsync(IEnumerable<Employee> employees, CancellationToken token);

    /// <summary>Массово сохранить транзакции в основную бизнес-таблицу.</summary>
    Task SaveTransactionsAsync(IEnumerable<Transaction> transactions, CancellationToken token);

    /// <summary>Получить все категории организации (для построения транзакций).</summary>
    Task<IEnumerable<Category>> GetCategoriesAsync(Guid organizationId, CancellationToken token);

    /// <summary>Получить всю номенклатуру организации (для построения транзакций).</summary>
    Task<IEnumerable<Nomenclature>> GetNomenclaturesAsync(Guid organizationId, CancellationToken token);

    /// <summary>Получить всех сотрудников организации (для построения транзакций).</summary>
    Task<IEnumerable<Employee>> GetEmployeesAsync(Guid organizationId, CancellationToken token);
}