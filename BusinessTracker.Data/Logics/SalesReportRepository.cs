using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Data.Logics;

/// <summary>
/// Реализация репозитория отчёта "Продажи".
/// Загружает транзакции из БД, преобразует в доменные модели,
/// затем строит отчёт через <see cref="SalesReportBuilder"/>.
/// </summary>
public class SalesReportRepository(BusinessTrackerContext context) : ISalesReportRepository
{
    public async Task<IEnumerable<SalesReportRowDto>> GetReport(
        Guid organizationId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        var entities = await context.Transactions
            .Include(t => t.Owner)
            .Include(t => t.Employee).ThenInclude(e => e.Owner)
            .Include(t => t.Nomenclature).ThenInclude(n => n.Category).ThenInclude(c => c.Owner)
            .Where(t => t.OwnerId == organizationId
                        && t.TransactionDate >= from.UtcDateTime
                        && t.TransactionDate <= to.UtcDateTime)
            .ToListAsync(cancellationToken);

        var domainTransactions = entities.Select(DomainMapper.ToDomain);
        return SalesReportBuilder.Build(domainTransactions);
    }
}