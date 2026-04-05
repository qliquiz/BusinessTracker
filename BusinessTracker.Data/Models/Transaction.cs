namespace BusinessTracker.Data.Models;

/// <summary>
///     EF-сущность транзакции.
/// </summary>
public class Transaction
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>Тип транзакции (см. <c>TransactionType</c> в Domain).</summary>
    public int Type { get; set; }

    /// <summary>Идентификатор организации-владельца.</summary>
    public Guid OwnerId { get; set; }

    /// <summary>Дата и время транзакции.</summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>Идентификатор номенклатуры.</summary>
    public Guid NomenclatureId { get; set; }

    /// <summary>Идентификатор сотрудника.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Сумма.</summary>
    public decimal Amount { get; set; }

    /// <summary>Количество.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Сумма скидки.</summary>
    public decimal Discount { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Nomenclature Nomenclature { get; set; } = null!;

    public virtual Organization Owner { get; set; } = null!;
}