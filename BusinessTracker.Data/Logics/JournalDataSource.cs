using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Data.Logics;

/// <summary>
///     Читает необработанные строки журнала из таблицы <c>JournalRows</c>.
/// </summary>
public class JournalDataSource : IJournalDataSource
{
    private readonly BusinessTrackerContext _context;

    public JournalDataSource(BusinessTrackerContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<JournalRowDto>> GetUnprocessedRowsAsync(
        Guid organizationId, long startPosition, int batchSize, CancellationToken token)
    {
        var rows = await _context.JournalRows
            .Where(r => r.OrganizationId == organizationId && r.Code >= startPosition)
            .OrderBy(r => r.Code)
            .Take(batchSize)
            .ToListAsync(token);

        return rows.Select(r => new JournalRowDto
        {
            Code = r.Code,
            TypeCode = r.TypeCode,
            TransTypeName = r.TransTypeName,
            ReceiptNumber = r.ReceiptNumber,
            ProductCode = r.ProductCode,
            CategoryCode = r.CategoryCode,
            EmployeeCode = r.EmployeeCode,
            Period = r.Period,
            Quantity = r.Quantity,
            Price = r.Price,
            Discount = r.Discount,
            RawId = r.RawId,
            RawLoginId = r.RawLoginId,
            EmployeeName = r.EmployeeName,
            CategoryName = r.CategoryName,
            NomenclatureName = r.NomenclatureName
        });
    }
}