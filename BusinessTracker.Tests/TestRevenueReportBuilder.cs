using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
/// Модульные тесты построителя отчёта "Выручка".
/// </summary>
public class TestRevenueReportBuilder
{
    private Organization _org = null!;
    private Employee _employee = null!;
    private Nomenclature _nomenclature = null!;

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
    /// Пустой список транзакций — пустой отчёт.
    /// </summary>
    [Test]
    public void Build_EmptyTransactions_ReturnsEmpty()
    {
        var report = RevenueReportBuilder.Build([]);

        Assert.That(report, Is.Empty);
    }

    /// <summary>
    /// Смены (StartShift / StopShift) не включаются в отчёт.
    /// </summary>
    [Test]
    public void Build_OnlyShiftTransactions_ReturnsEmpty()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, 0.01m, PaymentType.Cash),
            MakeTransaction(TransactionType.StopShift, 0.01m, PaymentType.Cash)
        };

        var report = RevenueReportBuilder.Build(transactions);

        Assert.That(report, Is.Empty);
    }

    /// <summary>
    /// Одна продажа наличными — корректно попадает в CashAmount.
    /// </summary>
    [Test]
    public void Build_SingleCashSale_CashAmountCorrect()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m, PaymentType.Cash, discount: 10m,
                date: new DateTimeOffset(2025, 3, 10, 12, 0, 0, TimeSpan.Zero))
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].CashAmount, Is.EqualTo(100m));
            Assert.That(report[0].NonCashAmount, Is.EqualTo(0m));
            Assert.That(report[0].OtherAmount, Is.EqualTo(0m));
            Assert.That(report[0].DiscountAmount, Is.EqualTo(10m));
            Assert.That(report[0].OrganizationId, Is.EqualTo(_org.Id));
        });
    }

    /// <summary>
    /// Несколько транзакций за один день группируются в одну строку.
    /// </summary>
    [Test]
    public void Build_MultipleTransactionsSameDay_GroupedIntoOneRow()
    {
        var date = new DateTimeOffset(2025, 3, 10, 0, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 200m, PaymentType.Cash, date: date),
            MakeTransaction(TransactionType.Sale, 300m, PaymentType.NonCash, date: date.AddHours(2)),
            MakeTransaction(TransactionType.Return, 50m, PaymentType.Cash, date: date.AddHours(4))
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].CashAmount, Is.EqualTo(250m));
            Assert.That(report[0].NonCashAmount, Is.EqualTo(300m));
        });
    }

    /// <summary>
    /// Транзакции за разные дни дают несколько строк.
    /// </summary>
    [Test]
    public void Build_TransactionsDifferentDays_MultipleRows()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m, PaymentType.Cash,
                date: new DateTimeOffset(2025, 3, 10, 12, 0, 0, TimeSpan.Zero)),
            MakeTransaction(TransactionType.Sale, 200m, PaymentType.Cash,
                date: new DateTimeOffset(2025, 3, 11, 12, 0, 0, TimeSpan.Zero))
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.That(report, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Выходной день помечается флагом IsHoliday = true.
    /// </summary>
    [Test]
    public void Build_SaturdayTransaction_IsHolidayTrue()
    {
        // 2025-03-08 — суббота
        var saturday = new DateTimeOffset(2025, 3, 8, 12, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m, PaymentType.Cash, date: saturday)
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.That(report[0].IsHoliday, Is.True);
    }

    /// <summary>
    /// Рабочий день не помечается как праздник.
    /// </summary>
    [Test]
    public void Build_WeekdayTransaction_IsHolidayFalse()
    {
        // 2025-03-10 — понедельник
        var monday = new DateTimeOffset(2025, 3, 10, 12, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, 100m, PaymentType.Cash, date: monday)
        };

        var report = RevenueReportBuilder.Build(transactions).ToList();

        Assert.That(report[0].IsHoliday, Is.False);
    }

    private Transaction MakeTransaction(
        TransactionType type,
        decimal amount,
        PaymentType paymentType,
        decimal discount = 0m,
        DateTimeOffset? date = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            PaymentType = paymentType,
            Amount = amount,
            Discount = discount,
            Quantity = 1m,
            TransactionDate = date ?? DateTimeOffset.UtcNow,
            Owner = _org,
            Employee = _employee,
            Nomenclature = _nomenclature
        };
}