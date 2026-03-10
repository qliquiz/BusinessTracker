using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public int TransactionType { get; set; }

    public Guid CompanyId { get; set; }

    public DateTime ChangePeriod { get; set; }

    public Guid? NomenclatureId { get; set; }

    public Guid? EmployeeId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Discount { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual Nomenclature? Nomenclature { get; set; }
}
