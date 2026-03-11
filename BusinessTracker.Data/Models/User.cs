namespace BusinessTracker.Data.Models;

/// <summary>
/// EF-сущность пользователя системы.
/// </summary>
public class User
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Имя пользователя.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Хэш пароля.</summary>
    public string Password { get; set; } = null!;

    public virtual ICollection<LinksUserOrganization> LinksUserOrganizations { get; set; } =
        new List<LinksUserOrganization>();
}