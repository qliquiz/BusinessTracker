using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Logic;

/// <summary>
///     Построитель отчёта "Продажи" на основе доменных моделей транзакций.
/// </summary>
public static class SalesReportBuilder
{
    private static readonly HashSet<TransactionType> SaleTypes =
        [TransactionType.Sale, TransactionType.Return];

    /// <summary>
    ///     Сформировать отчёт. Группировка — по номенклатуре.
    ///     Включает продажи и возвраты; суммирование ведётся суммарно.
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

    /// <summary>
    ///     Оптимизированная версия: один проход по коллекции через <see cref="Dictionary{TKey,TValue}" />.
    ///     Исключает создание промежуточных объектов <c>IGrouping</c> и повторные проходы
    ///     по каждой группе при вычислении сумм.
    /// </summary>
    public static IEnumerable<SalesReportRowDto> BuildOptimized(IEnumerable<Transaction> transactions)
    {
        // (catId, catName, nomName, qty, amount, discount, orgId) — весь контекст без промежуточных списков
        var groups = new Dictionary<Guid, (Guid CatId, string CatName, string NomName,
            decimal Qty, decimal Amount, decimal Discount, Guid OrgId)>();

        foreach (var t in transactions)
        {
            if (!SaleTypes.Contains(t.Type)) continue;

            var nomId = t.Nomenclature.Id;
            if (groups.TryGetValue(nomId, out var acc))
                groups[nomId] = (acc.CatId, acc.CatName, acc.NomName,
                    acc.Qty + t.Quantity, acc.Amount + t.Amount, acc.Discount + t.Discount, acc.OrgId);
            else
                groups[nomId] = (t.Nomenclature.Category.Id, t.Nomenclature.Category.Name,
                    t.Nomenclature.Name, t.Quantity, t.Amount, t.Discount, t.Owner.Id);
        }

        foreach (var (nomId, acc) in groups)
            yield return new SalesReportRowDto
            {
                CategoryId = acc.CatId,
                CategoryName = acc.CatName,
                NomenclatureId = nomId,
                NomenclatureName = acc.NomName,
                Quantity = acc.Qty,
                Amount = acc.Amount,
                DiscountAmount = acc.Discount,
                OrganizationId = acc.OrgId
            };
    }
}