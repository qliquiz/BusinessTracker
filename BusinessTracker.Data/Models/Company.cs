using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class Company
{
    public Guid Id { get; set; }

    public string? Inn { get; set; }

    public string? Address { get; set; }

    public string? LoadOptions { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<LinksUserCompany> LinksUserCompanies { get; set; } = new List<LinksUserCompany>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
