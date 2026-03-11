namespace BusinessTracker.Data.Models;

/// <summary>
/// EF-сущность организации.
/// </summary>
public class Organization
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Наименование.</summary>
    public string Name { get; set; } = null!;

    /// <summary>ИНН (10 цифр, уникальный).</summary>
    public string Inn { get; set; } = null!;

    /// <summary>Юридический адрес (формат КЛАДР).</summary>
    public string Address { get; set; } = null!;

    /// <summary>Настройки загрузки данных в формате JSON.</summary>
    public string? LoadOptions { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<LinksUserOrganization> LinksUserOrganizations { get; set; } =
        new List<LinksUserOrganization>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}