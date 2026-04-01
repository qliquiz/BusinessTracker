using BusinessTracker.Data.Models;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Data.Logics;

/// <summary>
/// Репозиторий для массовой записи строк журнала в таблицу JournalRows.
/// </summary>
public class JournalRowsRepository : IJournalRowsRepository
{
    private readonly BusinessTrackerContext _context;

    public JournalRowsRepository(BusinessTrackerContext context)
        => _context = context;

    /// <inheritdoc/>
    public async Task SaveAsync(Guid organizationId, IEnumerable<JournalRowDto> transactions, CancellationToken token)
    {
        var rows = transactions.Select(t => new JournalRow
        {
            OrganizationId   = organizationId,
            Code             = t.Code,
            TypeCode         = t.TypeCode,
            TransTypeName    = t.TransTypeName,
            ReceiptNumber    = t.ReceiptNumber,
            ProductCode      = t.ProductCode,
            CategoryCode     = t.CategoryCode,
            EmployeeCode     = t.EmployeeCode,
            Period           = DateTime.SpecifyKind(t.Period, DateTimeKind.Unspecified),
            Quantity         = t.Quantity,
            Price            = t.Price,
            Discount         = t.Discount,
            RawId            = t.RawId,
            RawLoginId       = t.RawLoginId,
            EmployeeName     = t.EmployeeName,
            CategoryName     = t.CategoryName,
            NomenclatureName = t.NomenclatureName
        }).ToList();

        await _context.JournalRows.AddRangeAsync(rows, token);
        await _context.SaveChangesAsync(token);
    }
}
