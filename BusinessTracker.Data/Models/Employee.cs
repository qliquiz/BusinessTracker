using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class Employee
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public Guid OwnerId { get; set; }

    public int Role { get; set; }

    public virtual Organization Owner { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
