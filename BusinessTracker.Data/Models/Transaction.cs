namespace BusinessTracker.Data.Models;

public class Transaction
{
    public Guid Id { get; set; }

    public int Type { get; set; }

    public Guid OwnerId { get; set; }

    public DateTime TransactionDate { get; set; }

    public Guid NomenclatureId { get; set; }

    public Guid EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public decimal Quantity { get; set; }

    public decimal Discount { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Nomenclature Nomenclature { get; set; } = null!;

    public virtual Organization Owner { get; set; } = null!;
}
