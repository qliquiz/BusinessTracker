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
}