using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class Nomenclature
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
