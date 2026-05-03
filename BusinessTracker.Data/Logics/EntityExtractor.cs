using BusinessTracker.Common.Core;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;
using DomainModels = BusinessTracker.Domain.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
///     Определяет новые сущности (категории, номенклатуру, сотрудников), сравнивая
///     данные из строк журнала с уже существующими записями в БД.
/// </summary>
public class EntityExtractor : IEntityExtractor
{
    private readonly BusinessTrackerContext _context;

    public EntityExtractor(BusinessTrackerContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Category>> ExtractNewCategoriesAsync(
        Guid organizationId, IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var existingNames = (await _context.Categories
                .Where(c => c.OwnerId == organizationId)
                .Select(c => c.Name)
                .ToListAsync(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var domainOrg = await LoadDomainOrgAsync(organizationId, token);

        return rows
            .Where(r => !string.IsNullOrWhiteSpace(r.CategoryName))
            .Select(r => r.CategoryName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(name => !existingNames.Contains(name))
            .Select(name => new DomainModels.Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Owner = domainOrg
            });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Nomenclature>> ExtractNewNomenclatureAsync(
        Guid organizationId, IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var existingNames = (await _context.Nomenclatures
                .Where(n => n.Category.OwnerId == organizationId)
                .Select(n => n.Name)
                .ToListAsync(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var categories = await _context.Categories
            .Where(c => c.OwnerId == organizationId)
            .ToListAsync(token);
        var categoryByName = categories.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

        var domainOrg = await LoadDomainOrgAsync(organizationId, token);

        var rowsList = rows.ToList();
        var result = new List<DomainModels.Nomenclature>();

        var newNomNames = rowsList
            .Where(r => !string.IsNullOrWhiteSpace(r.NomenclatureName))
            .Select(r => r.NomenclatureName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(name => !existingNames.Contains(name));

        foreach (var nomName in newNomNames)
        {
            var catName = rowsList
                .First(r => r.NomenclatureName.Equals(nomName, StringComparison.OrdinalIgnoreCase))
                .CategoryName;

            if (!categoryByName.TryGetValue(catName ?? string.Empty, out var dataCategory)) continue;

            result.Add(new DomainModels.Nomenclature
            {
                Id = Guid.NewGuid(),
                Name = nomName,
                Category = new DomainModels.Category
                {
                    Id = dataCategory.Id,
                    Name = dataCategory.Name,
                    Owner = domainOrg
                }
            });
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Employee>> ExtractNewEmployeesAsync(
        Guid organizationId, IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var existingNames = (await _context.Employees
                .Where(e => e.OwnerId == organizationId)
                .Select(e => e.Name)
                .ToListAsync(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var domainOrg = await LoadDomainOrgAsync(organizationId, token);

        return rows
            .Where(r => !string.IsNullOrWhiteSpace(r.EmployeeName))
            .Select(r => r.EmployeeName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(name => !existingNames.Contains(name))
            .Select(name => new DomainModels.Employee
            {
                Id = Guid.NewGuid(),
                Name = name,
                Owner = domainOrg
            });
    }

    private async Task<DomainModels.Organization> LoadDomainOrgAsync(Guid organizationId, CancellationToken token)
    {
        var org = await _context.Organizations
                      .FirstOrDefaultAsync(o => o.Id == organizationId, token)
                  ?? throw new InvalidOperationException($"Organization {organizationId} not found");

        return new DomainModels.Organization
        {
            Id = org.Id,
            Name = org.Name,
            Inn = org.Inn,
            Address = org.Address
        };
    }
}