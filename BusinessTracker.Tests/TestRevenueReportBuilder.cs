using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
///     Модульные тесты построителя отчёта "Выручка".
/// </summary>
public class TestRevenueReportBuilder
{
    private Employee _employee = null!;
    private Nomenclature _nomenclature = null!;
    private Organization _org = null!;

    [SetUp]
    public void SetUp()
    {
        _org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Test Org",
            Inn = "0123456789",
            Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
        };
        _employee = new Employee { Id = Guid.NewGuid(), Name = "Иванов И.И.", Owner = _org };
        var category = new Category { Id = Guid.NewGuid(), Name = "Food", Owner = _org };
        _nomenclature = new Nomenclature { Id = Guid.NewGuid(), Name = "Bread", Category = category };
    }

    /// <summary>
    ///     Пустой список транзакций — пустой отчёт.
    /// </summary>
    [Test]
    public void Build_EmptyTransactions_ReturnsEmpty()
    {
        Assert.That(RevenueReportBuilder.Build([]), Is.Empty);
    }

    /// <summary>
    ///     Смены (StartShift / StopShift) не включаются в отчёт.
    /// </summary>
    [Test]
    public void Build_OnlyShiftTransactions_ReturnsEmpty()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, 0.01m),
            MakeTransaction(TransactionType.StopShift, 0.01m)
        };

        Assert.That(RevenueReportBuilder.Build(transactions), Is.Empty);
    }

    /// <summary>
    ///     Продажа попадает в отчёт с корректной суммой.
    /// </summary>
    [Test]
    public void Build_SingleSale_AmountCorrect()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m, 10m,
                new DateTimeOffset(2025, 3, 10, 12, 0, 0, TimeSpan.Zero))
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].CashAmount, Is.EqualTo(100m));
            Assert.That(report[0].DiscountAmount, Is.EqualTo(10m));
            Assert.That(report[0].OrganizationId, Is.EqualTo(_org.Id));
        });
    }

    /// <summary>
    ///     Несколько транзакций за один день группируются в одну строку.
    /// </summary>
    [Test]
    public void Build_MultipleTransactionsSameDay_GroupedIntoOneRow()
    {
        var date = new DateTimeOffset(2025, 3, 10, 0, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 200m, date: date),
            MakeTransaction(TransactionType.Sale, 300m, date: date.AddHours(2)),
            MakeTransaction(TransactionType.Return, 50m, date: date.AddHours(4))
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].CashAmount, Is.EqualTo(550m));
        });
    }

    /// <summary>
    ///     Транзакции за разные дни дают несколько строк.
    /// </summary>
    [Test]
    public void Build_TransactionsDifferentDays_MultipleRows()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m,
                date: new DateTimeOffset(2025, 3, 10, 12, 0, 0, TimeSpan.Zero)),
            MakeTransaction(TransactionType.Sale, 200m,
                date: new DateTimeOffset(2025, 3, 11, 12, 0, 0, TimeSpan.Zero))
        };

        Assert.That(RevenueReportBuilder.Build(transactions).Count(), Is.EqualTo(2));
    }

    /// <summary>
    ///     День из переданного набора праздников помечается IsHoliday = true.
    /// </summary>
    [Test]
    public void Build_DateInHolidaySet_IsHolidayTrue()
    {
        var date = new DateOnly(2025, 1, 1); // Новый год
        var holidays = new HashSet<DateOnly> { date };
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m,
                date: new DateTimeOffset(date.Year, date.Month, date.Day, 12, 0, 0, TimeSpan.Zero))
        };

        var report = RevenueReportBuilder.Build(transactions, holidays).ToList();

        Assert.That(report[0].IsHoliday, Is.True);
    }

    /// <summary>
    ///     День не из набора праздников — IsHoliday = false.
    /// </summary>
    [Test]
    public void Build_DateNotInHolidaySet_IsHolidayFalse()
    {
        var holidays = new HashSet<DateOnly> { new(2025, 1, 1) };
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m,
                date: new DateTimeOffset(2025, 3, 10, 12, 0, 0, TimeSpan.Zero))
        };

        var report = RevenueReportBuilder.Build(transactions, holidays).ToList();

        Assert.That(report[0].IsHoliday, Is.False);
    }

    /// <summary>
    ///     Без переданного набора праздников IsHoliday всегда false.
    /// </summary>
    [Test]
    public void Build_NoHolidaysProvided_IsHolidayAlwaysFalse()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m,
                date: new DateTimeOffset(2025, 3, 8, 12, 0, 0, TimeSpan.Zero)) // воскресенье
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.That(report[0].IsHoliday, Is.False);
    }

    private Transaction MakeTransaction(
        TransactionType type,
        decimal amount,
        decimal discount = 0m,
        DateTimeOffset? date = null)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Type = type,
            Amount = amount,
            Discount = discount,
            Quantity = 1m,
            TransactionDate = date ?? DateTimeOffset.UtcNow,
            Owner = _org,
            Employee = _employee,
            Nomenclature = _nomenclature
        };
    }
}