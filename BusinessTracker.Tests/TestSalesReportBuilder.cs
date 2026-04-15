using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
///     Модульные тесты построителя отчёта "Продажи".
/// </summary>
public class TestSalesReportBuilder
{
    private Nomenclature _bread = null!;
    private Category _category = null!;
    private Employee _employee = null!;
    private Nomenclature _milk = null!;
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
        _category = new Category { Id = Guid.NewGuid(), Name = "Продукты", Owner = _org };
        _bread = new Nomenclature { Id = Guid.NewGuid(), Name = "Хлеб", Category = _category };
        _milk = new Nomenclature { Id = Guid.NewGuid(), Name = "Молоко", Category = _category };
    }

    /// <summary>
    ///     Пустой список — пустой отчёт.
    /// </summary>
    [Test]
    public void Build_EmptyTransactions_ReturnsEmpty()
    {
        Assert.That(SalesReportBuilder.Build([]), Is.Empty);
    }

    /// <summary>
    ///     Смены не попадают в отчёт продаж.
    /// </summary>
    [Test]
    public void Build_ShiftTransactions_Excluded()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _bread, 0.01m),
            MakeTransaction(TransactionType.StopShift, _bread, 0.01m)
        };

        Assert.That(SalesReportBuilder.Build(transactions), Is.Empty);
    }

    /// <summary>
    ///     Две продажи одной номенклатуры группируются в одну строку.
    /// </summary>
    [Test]
    public void Build_TwoSalesSameNomenclature_OneRow()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, _bread, 50m, 1m, 5m),
            MakeTransaction(TransactionType.Sale, _bread, 50m, 2m, 5m)
        };

        var report = SalesReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].NomenclatureId, Is.EqualTo(_bread.Id));
            Assert.That(report[0].NomenclatureName, Is.EqualTo(_bread.Name));
            Assert.That(report[0].CategoryId, Is.EqualTo(_category.Id));
            Assert.That(report[0].CategoryName, Is.EqualTo(_category.Name));
            Assert.That(report[0].Quantity, Is.EqualTo(3m));
            Assert.That(report[0].Amount, Is.EqualTo(100m));
            Assert.That(report[0].DiscountAmount, Is.EqualTo(10m));
            Assert.That(report[0].OrganizationId, Is.EqualTo(_org.Id));
        });
    }

    /// <summary>
    ///     Разные номенклатуры дают разные строки.
    /// </summary>
    [Test]
    public void Build_DifferentNomenclatures_MultipleRows()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, _bread, 100m),
            MakeTransaction(TransactionType.Sale, _milk, 80m)
        };

        Assert.That(SalesReportBuilder.Build(transactions).Count(), Is.EqualTo(2));
    }

    /// <summary>
    ///     Возвраты включаются в отчёт наравне с продажами (суммирование валовое).
    /// </summary>
    [Test]
    public void Build_ReturnIncludedInReport()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, _bread, 100m, 5m),
            MakeTransaction(TransactionType.Return, _bread, 20m, 1m)
        };

        var report = SalesReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].Quantity, Is.EqualTo(6m));
            Assert.That(report[0].Amount, Is.EqualTo(120m));
        });
    }

    private Transaction MakeTransaction(
        TransactionType type,
        Nomenclature nomenclature,
        decimal amount = 1m,
        decimal quantity = 1m,
        decimal discount = 0m)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Type = type,
            Amount = amount,
            Discount = discount,
            Quantity = quantity,
            TransactionDate = DateTimeOffset.UtcNow,
            Owner = _org,
            Employee = _employee,
            Nomenclature = nomenclature
        };
    }
}