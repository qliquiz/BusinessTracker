namespace BusinessTracker.Data.Models;

/// <summary>
///     EF-сущность связи пользователя с организацией.
/// </summary>
public class LinksUserOrganization
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Идентификатор пользователя.</summary>
    public Guid UserId { get; set; }

    /// <summary>Идентификатор организации.</summary>
    public Guid OrganizationId { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}