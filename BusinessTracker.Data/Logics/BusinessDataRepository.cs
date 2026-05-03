using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using DataModels = BusinessTracker.Data.Models;
using DomainModels = BusinessTracker.Domain.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
///     Сохраняет нормализованные бизнес-данные и предоставляет справочники для построения транзакций.
/// </summary>
public class BusinessDataRepository : IBusinessDataRepository
{
    private readonly BusinessTrackerContext _context;

    public BusinessDataRepository(BusinessTrackerContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task SaveCategoriesAsync(IEnumerable<DomainModels.Category> categories, CancellationToken token)
    {
        var entities = categories.Select(c => new DataModels.Category
        {
            Id = c.Id,
            Name = c.Name,
            OwnerId = c.Owner.Id
        });
        await _context.Categories.AddRangeAsync(entities, token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task SaveNomenclatureAsync(IEnumerable<DomainModels.Nomenclature> nomenclatures,
        CancellationToken token)
    {
        var entities = nomenclatures.Select(n => new DataModels.Nomenclature
        {
            Id = n.Id,
            Name = n.Name,
            CategoryId = n.Category.Id
        });
        await _context.Nomenclatures.AddRangeAsync(entities, token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task SaveEmployeesAsync(IEnumerable<DomainModels.Employee> employees, CancellationToken token)
    {
        var entities = employees.Select(e => new DataModels.Employee
        {
            Id = e.Id,
            Name = e.Name,
            PhoneNumber = e.PhoneNumber,
            OwnerId = e.Owner.Id,
            Role = (int)e.Role
        });
        await _context.Employees.AddRangeAsync(entities, token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task SaveTransactionsAsync(IEnumerable<DomainModels.Transaction> transactions,
        CancellationToken token)
    {
        var entities = transactions.Select(t => new DataModels.Transaction
        {
            Id = t.Id,
            Type = (int)t.Type,
            OwnerId = t.Owner.Id,
            TransactionDate = t.TransactionDate.DateTime,
            NomenclatureId = t.Nomenclature.Id,
            EmployeeId = t.Employee.Id,
            Amount = t.Amount,
            Quantity = t.Quantity,
            Discount = t.Discount
        });
        await _context.Transactions.AddRangeAsync(entities, token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Category>> GetCategoriesAsync(Guid organizationId,
        CancellationToken token)
    {
        var domainOrg = await LoadDomainOrgAsync(organizationId, token);

        var data = await _context.Categories
            .Where(c => c.OwnerId == organizationId)
            .ToListAsync(token);

        return data.Select(c => new DomainModels.Category
        {
            Id = c.Id,
            Name = c.Name,
            Owner = domainOrg
        });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Nomenclature>> GetNomenclaturesAsync(Guid organizationId,
        CancellationToken token)
    {
        var domainOrg = await LoadDomainOrgAsync(organizationId, token);

        var data = await _context.Nomenclatures
            .Include(n => n.Category)
            .Where(n => n.Category.OwnerId == organizationId)
            .ToListAsync(token);

        return data.Select(n => new DomainModels.Nomenclature
        {
            Id = n.Id,
            Name = n.Name,
            Category = new DomainModels.Category
            {
                Id = n.Category.Id,
                Name = n.Category.Name,
                Owner = domainOrg
            }
        });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Employee>> GetEmployeesAsync(Guid organizationId,
        CancellationToken token)
    {
        var domainOrg = await LoadDomainOrgAsync(organizationId, token);

        var data = await _context.Employees
            .Where(e => e.OwnerId == organizationId)
            .ToListAsync(token);

        return data.Select(e => new DomainModels.Employee
        {
            Id = e.Id,
            Name = e.Name,
            PhoneNumber = e.PhoneNumber,
            Owner = domainOrg,
            Role = (EmployeeRole)e.Role
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