namespace BusinessTracker.Data.Models;

/// <summary>
/// EF-сущность сотрудника.
/// </summary>
public class Employee
{
    /// <summary>Уникальный идентификатор.</summary>
    public Guid Id { get; set; }

    /// <summary>ФИО.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Контактный телефон (необязательно).</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Идентификатор организации-владельца.</summary>
    public Guid OwnerId { get; set; }

    /// <summary>Роль сотрудника (0 — Manager, 1 — Administrator).</summary>
    public int Role { get; set; }

    public virtual Organization Owner { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}