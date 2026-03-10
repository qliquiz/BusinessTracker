using System;
using System.Collections.Generic;

namespace BusinessTracker.Data.Models;

public partial class LinksUserOrganization
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid OrganizationId { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
