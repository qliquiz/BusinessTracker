namespace BusinessTracker.Data.Models;

/// <summary>
///     EF-сущность номенклатуры (товара/услуги).
/// </summary>
public class Nomenclature
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Наименование.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Идентификатор категории.</summary>
    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}