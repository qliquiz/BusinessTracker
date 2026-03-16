using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Logic;

/// <summary>
/// Построитель отчёта "Выручка" на основе доменных моделей транзакций.
/// </summary>
public static class RevenueReportBuilder
{
    private static readonly HashSet<TransactionType> RevenueTypes =
        [TransactionType.Sale, TransactionType.Return];

    /// <summary>
    /// Сформировать отчёт. Группировка — по дате (один день = одна строка).
    /// <para>
    /// Разбивка по типам оплаты (наличные / безналичные / прочее) будет реализована
    /// после интеграции полной таблицы типов транзакций из MSSQL-журнала (transtype).
    /// На данный момент вся сумма попадает в <see cref="RevenueReportRowDto.CashAmount"/>.
    /// </para>
    /// </summary>
    /// <param name="transactions">Набор доменных транзакций.</param>
    /// <param name="holidays">
    /// Набор дат, считающихся праздничными (выходные, государственные праздники и т.д.).
    /// Если не передан — <see cref="RevenueReportRowDto.IsHoliday"/> всегда <c>false</c>.
    /// </param>
    public static IEnumerable<RevenueReportRowDto> Build(
        IEnumerable<Transaction> transactions,
        IReadOnlySet<DateOnly>? holidays = null)
    {
        return transactions
            .Where(t => RevenueTypes.Contains(t.Type))
            .GroupBy(t => DateOnly.FromDateTime(t.TransactionDate.LocalDateTime))
            .Select(g => new RevenueReportRowDto
            {
                Period = g.Key,
                CashAmount = g.Sum(t => t.Amount),
                NonCashAmount = 0m,
                OtherAmount = 0m,
                DiscountAmount = g.Sum(t => t.Discount),
                IsHoliday = holidays?.Contains(g.Key) ?? false,
                OrganizationId = g.First().Owner.Id
            });
    }

    /// <summary>
    /// Оптимизированная версия: один проход по коллекции через <see cref="Dictionary{TKey,TValue}"/>.
    /// Исключает создание промежуточных объектов <c>IGrouping</c> и повторные проходы
    /// по каждой группе при вычислении сумм.
    /// </summary>
    /// <param name="transactions">Набор доменных транзакций.</param>
    /// <param name="holidays">
    /// Набор дат, считающихся праздничными.
    /// Если не передан — <see cref="RevenueReportRowDto.IsHoliday"/> всегда <c>false</c>.
    /// </param>
    public static IEnumerable<RevenueReportRowDto> BuildOptimized(
        IEnumerable<Transaction> transactions,
        IReadOnlySet<DateOnly>? holidays = null)
    {
        // (cash, discount, orgId) накапливаются без создания промежуточных коллекций
        var groups = new Dictionary<DateOnly, (decimal Cash, decimal Discount, Guid OrgId)>();

        foreach (var t in transactions)
        {
            if (!RevenueTypes.Contains(t.Type)) continue;

            var date = DateOnly.FromDateTime(t.TransactionDate.LocalDateTime);
            if (groups.TryGetValue(date, out var acc))
                groups[date] = (acc.Cash + t.Amount, acc.Discount + t.Discount, acc.OrgId);
            else
                groups[date] = (t.Amount, t.Discount, t.Owner.Id);
        }

        foreach (var (date, acc) in groups)
        {
            yield return new RevenueReportRowDto
            {
                Period = date,
                CashAmount = acc.Cash,
                NonCashAmount = 0m,
                OtherAmount = 0m,
                DiscountAmount = acc.Discount,
                IsHoliday = holidays?.Contains(date) ?? false,
                OrganizationId = acc.OrgId
            };
        }
    }
}