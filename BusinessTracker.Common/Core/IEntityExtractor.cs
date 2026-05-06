using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Common.Core;

/// <summary>
///     Сервис выделения новых бизнес-сущностей из потока сырых строк журнала.
///     Сравнивает данные в <see cref="JournalRowDto" /> с уже сохранёнными сущностями
///     и возвращает только те, которые отсутствуют в БД.
/// </summary>
public interface IEntityExtractor
{
    /// <summary>
    ///     Извлекает категории номенклатуры, которых ещё нет в БД для указанной организации.
    /// </summary>
    Task<IEnumerable<Category>> ExtractNewCategoriesAsync(Guid organizationId, IEnumerable<JournalRowDto> rows,
        CancellationToken token);

    /// <summary>
    ///     Извлекает позиции номенклатуры (товары/услуги), которых ещё нет в БД.
    ///     Вызывать после <see cref="ExtractNewCategoriesAsync" /> и сохранения категорий.
    /// </summary>
    Task<IEnumerable<Nomenclature>> ExtractNewNomenclatureAsync(Guid organizationId, IEnumerable<JournalRowDto> rows,
        CancellationToken token);

    /// <summary>
    ///     Извлекает сотрудников, которых ещё нет в БД для указанной организации.
    /// </summary>
    Task<IEnumerable<Employee>> ExtractNewEmployeesAsync(Guid organizationId, IEnumerable<JournalRowDto> rows,
        CancellationToken token);
}