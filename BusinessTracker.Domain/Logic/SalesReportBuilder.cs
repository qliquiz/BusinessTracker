using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Logic;

/// <summary>
/// Построитель отчёта "Продажи" на основе доменных моделей транзакций.
/// </summary>
public static class SalesReportBuilder
{
    private static readonly HashSet<TransactionType> SaleTypes =
        [TransactionType.Sale, TransactionType.Return];

    /// <summary>
    /// Сформировать отчёт. Группировка — по номенклатуре.
    /// Включает продажи и возвраты; суммирование ведётся суммарно.
    /// </summary>
    public static IEnumerable<SalesReportRowDto> Build(IEnumerable<Transaction> transactions)
    {
        return transactions
            .Where(t => SaleTypes.Contains(t.Type))
            .GroupBy(t => t.Nomenclature.Id)
            .Select(g =>
            {
                var first = g.First();
                return new SalesReportRowDto
                {
                    CategoryId = first.Nomenclature.Category.Id,
                    CategoryName = first.Nomenclature.Category.Name,
                    NomenclatureId = first.Nomenclature.Id,
                    NomenclatureName = first.Nomenclature.Name,
                    Quantity = g.Sum(t => t.Quantity),
                    Amount = g.Sum(t => t.Amount),
                    DiscountAmount = g.Sum(t => t.Discount),
                    OrganizationId = first.Owner.Id
                };
            });
    }
}