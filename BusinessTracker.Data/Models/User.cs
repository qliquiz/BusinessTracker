using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<LinksUserOrganization> LinksUserOrganizations { get; set; } = new List<LinksUserOrganization>();
}
