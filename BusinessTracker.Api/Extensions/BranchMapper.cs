using BusinessTracker.Domain.Models;

namespace BusinessTracker.Api.Extensions;

/// <summary>
///     Mapper для филиалов.
/// </summary>
public static class BranchMapper
{
    public static Branch MapBranch(Data.Models.Branch entity)
    {
        return new Branch
        {
            Id = entity.Id,
            Name = entity.Name,
            Owner = new Organization
            {
                Id = entity.Owner.Id,
                Name = entity.Owner.Name,
                Inn = entity.Owner.Inn,
                Address = entity.Owner.Address
            }
        };
    }
}