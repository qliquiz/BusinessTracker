using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Models.Dto;

/// <summary>
/// Записть в журнале в клиенсткой программе.
/// </summary>
public class JournalRowDto : IDto
{
    /// <summary>
    /// Уникальный код транзакции.
    /// </summary>
    public long Code { get; set; }

    /// <summary>
    /// Уникальный код типа транзакции.
    /// </summary>
    public long TypeCode { get; set; }

    /// <summary>
    /// Номер чека.
    /// </summary>
    public long ReceiptNumber { get; set; }

    /// <summary>
    /// Уникальный код продукта.
    /// </summary>
    public long ProductCode { get; set; }

    /// <summary>
    /// Уникальный код категории продуктов.
    /// </summary>
    public long CategoryCode { get; set; }

    /// <summary>
    /// Код сотрудника.
    /// </summary>
    public long EmployeeCode { get; set; }

    /// <summary>
    /// Дата время транзакции.
    /// </summary>
    public DateTimeOffset Period { get; set; }

    /// <summary>
    /// Количество.
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    public double Discount { get; set; }

    public override string ToString() => $"{Period}:Транзакция - {Quantity * Price}";
}