namespace BusinessTracker.Data.Models;

/// <summary>
/// EF-сущность категории номенклатуры.
/// </summary>
public class Category
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Наименование.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Идентификатор организации-владельца.</summary>
    public Guid OwnerId { get; set; }

    public virtual ICollection<Nomenclature> Nomenclatures { get; set; } = new List<Nomenclature>();

    public virtual Organization Owner { get; set; } = null!;
}