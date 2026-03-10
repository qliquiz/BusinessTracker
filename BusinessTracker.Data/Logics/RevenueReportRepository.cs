using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Data.Logics;

/// <summary>
/// Реализация репозитория отчёта "Выручка".
/// Загружает транзакции из БД, преобразует в доменные модели,
/// затем строит отчёт через <see cref="RevenueReportBuilder"/>.
/// </summary>
public class RevenueReportRepository(BusinessTrackerContext context) : IRevenueReportRepository
{
    public async Task<IEnumerable<RevenueReportRowDto>> GetReport(
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
        return RevenueReportBuilder.Build(domainTransactions);
    }
}