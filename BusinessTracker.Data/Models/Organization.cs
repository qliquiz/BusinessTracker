using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class Organization
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Inn { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? LoadOptions { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<LinksUserOrganization> LinksUserOrganizations { get; set; } = new List<LinksUserOrganization>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
