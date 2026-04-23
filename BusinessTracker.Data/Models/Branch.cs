namespace BusinessTracker.Data.Models;

/// <summary>
///     EF-сущность филиала организации.
/// </summary>
public class Branch
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Наименование.</summary>
    public string Name { get; set; } = null!;

    /// <summary>FK на организацию-владельца.</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>Настройки загрузки данных в формате JSON.</summary>
    public string? LoadOptions { get; set; }

    public virtual Organization Owner { get; set; } = null!;
}