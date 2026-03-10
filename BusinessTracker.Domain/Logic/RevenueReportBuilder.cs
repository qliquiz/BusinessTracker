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
    /// Выходные и праздники помечаются флагом <see cref="RevenueReportRowDto.IsHoliday"/>.
    /// </summary>
    public static IEnumerable<RevenueReportRowDto> Build(IEnumerable<Transaction> transactions)
    {
        return transactions
            .Where(t => RevenueTypes.Contains(t.Type))
            .GroupBy(t => DateOnly.FromDateTime(t.TransactionDate.LocalDateTime))
            .Select(g => new RevenueReportRowDto
            {
                Period = g.Key,
                CashAmount = g
                    .Where(t => t.PaymentType == PaymentType.Cash)
                    .Sum(t => t.Amount),
                NonCashAmount = g
                    .Where(t => t.PaymentType == PaymentType.NonCash)
                    .Sum(t => t.Amount),
                OtherAmount = g
                    .Where(t => t.PaymentType == PaymentType.Other)
                    .Sum(t => t.Amount),
                DiscountAmount = g.Sum(t => t.Discount),
                IsHoliday = g.Key.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
                OrganizationId = g.First().Owner.Id
            });
    }
}