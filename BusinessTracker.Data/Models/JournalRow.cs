namespace BusinessTracker.Data.Models;

/// <summary>
/// Плоская строка журнала транзакций, сохранённая из legacy MSSQL.
/// </summary>
public class JournalRow
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public long Code { get; set; }

    public int TypeCode { get; set; }

    public string TransTypeName { get; set; } = string.Empty;

    public int ReceiptNumber { get; set; }

    public long? ProductCode { get; set; }

    public long? CategoryCode { get; set; }

    public int? EmployeeCode { get; set; }

    public DateTime Period { get; set; }

    public double Quantity { get; set; }

    public double Price { get; set; }

    public double Discount { get; set; }

    public int RawId { get; set; }

    public int RawLoginId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string NomenclatureName { get; set; } = string.Empty;

    public Organization? Owner { get; set; }
}
