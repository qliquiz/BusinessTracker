using DomainEnums = BusinessTracker.Domain.Core.Enums;
using DomainModels = BusinessTracker.Domain.Models;
using DataModels = BusinessTracker.Data.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
/// Маппер из EF-сущностей в доменные модели.
/// </summary>
internal static class DomainMapper
{
    private static DomainModels.Organization ToDomain(DataModels.Organization entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Inn = entity.Inn,
            Address = entity.Address
        };

    private static DomainModels.Category ToDomain(DataModels.Category entity, DomainModels.Organization owner) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Owner = owner
        };

    private static DomainModels.Employee ToDomain(DataModels.Employee entity, DomainModels.Organization owner) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            PhoneNumber = entity.PhoneNumber,
            Role = (DomainEnums.EmployeeRole)entity.Role,
            Owner = owner
        };

    private static DomainModels.Nomenclature
        ToDomain(DataModels.Nomenclature entity, DomainModels.Category category) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Category = category
        };

    private static DomainModels.Transaction ToDomain(
        DataModels.Transaction entity,
        DomainModels.Organization owner,
        DomainModels.Nomenclature nomenclature,
        DomainModels.Employee employee) =>
        new()
        {
            Id = entity.Id,
            Type = (DomainEnums.TransactionType)entity.Type,
            PaymentType = (DomainEnums.PaymentType)entity.PaymentType,
            Amount = entity.Amount,
            Discount = entity.Discount,
            Quantity = entity.Quantity,
            TransactionDate = new DateTimeOffset(entity.TransactionDate, TimeSpan.Zero),
            Owner = owner,
            Nomenclature = nomenclature,
            Employee = employee
        };

    internal static DomainModels.Transaction ToDomain(DataModels.Transaction entity) =>
        ToDomain(
            entity,
            owner: ToDomain(entity.Owner),
            nomenclature: ToDomain(
                entity.Nomenclature,
                category: ToDomain(entity.Nomenclature.Category, ToDomain(entity.Nomenclature.Category.Owner))),
            employee: ToDomain(entity.Employee, ToDomain(entity.Employee.Owner)));
}