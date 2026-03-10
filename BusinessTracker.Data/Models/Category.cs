using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public virtual ICollection<Nomenclature> Nomenclatures { get; set; } = new List<Nomenclature>();

    public virtual Organization Owner { get; set; } = null!;
}
