using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель транзакции.
/// </summary>
public class Transaction : BaseEntity
{
    /// <summary>
    /// Сумма.
    /// </summary>
    [Required]
    public required decimal Amount { get; set; }

    public Guid OrganizationId { get; set; }
    public virtual Organization? Organization { get; set; }

    public Guid NomenclatureId { get; set; }
    public virtual Nomenclature? Nomenclature { get; set; }

    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
}