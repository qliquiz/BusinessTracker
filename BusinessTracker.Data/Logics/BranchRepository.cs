using BusinessTracker.Domain.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using DomainModels = BusinessTracker.Domain.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
///     Реализация <see cref="IBranchRepository" /> через EF Core.
/// </summary>
public class BranchRepository : IBranchRepository
{
    private readonly BusinessTrackerContext _context;

    public BranchRepository(BusinessTrackerContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainModels.Branch>> GetAllAsync(CancellationToken cancellationToken)
    {
        var branches = await _context.Branches
            .Include(b => b.Owner)
            .ToListAsync(cancellationToken);

        return branches.Select(b => new DomainModels.Branch
        {
            Id = b.Id,
            Name = b.Name,
            Owner = new DomainModels.Organization
            {
                Id = b.Owner.Id,
                Name = b.Owner.Name,
                Inn = b.Owner.Inn,
                Address = b.Owner.Address
            }
        });
    }
}