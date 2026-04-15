using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
///     Модульные тесты построителя отчёта "График работы".
/// </summary>
public class TestWorkScheduleReportBuilder
{
    private Employee _employeeA = null!;
    private Employee _employeeB = null!;
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
        _employeeA = new Employee { Id = Guid.NewGuid(), Name = "Иванов И.И.", Owner = _org };
        _employeeB = new Employee { Id = Guid.NewGuid(), Name = "Петров П.П.", Owner = _org };
        var category = new Category { Id = Guid.NewGuid(), Name = "Food", Owner = _org };
        _nomenclature = new Nomenclature { Id = Guid.NewGuid(), Name = "Bread", Category = category };
    }

    /// <summary>
    ///     Пустой список — пустой отчёт.
    /// </summary>
    [Test]
    public void Build_EmptyTransactions_ReturnsEmpty()
    {
        Assert.That(WorkScheduleReportBuilder.Build([]), Is.Empty);
    }

    /// <summary>
    ///     Продажи не попадают в отчёт графика работы.
    /// </summary>
    [Test]
    public void Build_SaleTransactions_Excluded()
    {
        var transactions = new[]
        {
            MakeTransaction(TransactionType.Sale, _employeeA, DateTimeOffset.UtcNow)
        };

        Assert.That(WorkScheduleReportBuilder.Build(transactions), Is.Empty);
    }

    /// <summary>
    ///     Одна открытая смена (нет StopShift) — строка с ShiftEnd == null.
    /// </summary>
    [Test]
    public void Build_StartShiftWithoutStop_ShiftEndIsNull()
    {
        var start = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, start)
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].EmployeeId, Is.EqualTo(_employeeA.Id));
            Assert.That(report[0].ShiftStart, Is.EqualTo(start));
            Assert.That(report[0].ShiftEnd, Is.Null);
        });
    }

    /// <summary>
    ///     Закрытая смена корректно связывает StartShift и StopShift.
    /// </summary>
    [Test]
    public void Build_ClosedShift_ShiftEndMatchesStopShift()
    {
        var start = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var stop = new DateTimeOffset(2025, 3, 10, 18, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, start),
            MakeTransaction(TransactionType.StopShift, _employeeA, stop)
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].ShiftStart, Is.EqualTo(start));
            Assert.That(report[0].ShiftEnd, Is.EqualTo(stop));
        });
    }

    /// <summary>
    ///     Два сотрудника — каждый получает свою строку.
    /// </summary>
    [Test]
    public void Build_TwoEmployees_TwoRows()
    {
        var base_ = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, base_),
            MakeTransaction(TransactionType.StopShift, _employeeA, base_.AddHours(8)),
            MakeTransaction(TransactionType.StartShift, _employeeB, base_.AddHours(1)),
            MakeTransaction(TransactionType.StopShift, _employeeB, base_.AddHours(9))
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(2));
            Assert.That(report.Select(r => r.EmployeeId), Does.Contain(_employeeA.Id));
            Assert.That(report.Select(r => r.EmployeeId), Does.Contain(_employeeB.Id));
        });
    }

    /// <summary>
    ///     StopShift одного сотрудника не связывается со StartShift другого.
    /// </summary>
    [Test]
    public void Build_StopShiftBelongsToCorrectEmployee()
    {
        var base_ = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, base_),
            MakeTransaction(TransactionType.StopShift, _employeeB, base_.AddHours(8))
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        // StopShift сотрудника B не закрывает StartShift сотрудника A
        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].EmployeeId, Is.EqualTo(_employeeA.Id));
            Assert.That(report[0].ShiftEnd, Is.Null);
        });
    }

    /// <summary>
    ///     Два StartShift подряд без StopShift — две открытые смены.
    /// </summary>
    [Test]
    public void Build_TwoStartsNoStop_TwoOpenShifts()
    {
        var base_ = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, base_),
            MakeTransaction(TransactionType.StartShift, _employeeA, base_.AddHours(1))
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(2));
            Assert.That(report.All(r => r.ShiftEnd == null), Is.True);
        });
    }

    /// <summary>
    ///     Один StartShift и два StopShift — первый StopShift закрывает смену,
    ///     второй (осиротевший) игнорируется.
    /// </summary>
    [Test]
    public void Build_OneStartTwoStops_OneShiftClosedOrphanIgnored()
    {
        var base_ = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, base_),
            MakeTransaction(TransactionType.StopShift, _employeeA, base_.AddHours(8)),
            MakeTransaction(TransactionType.StopShift, _employeeA, base_.AddHours(9))
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(1));
            Assert.That(report[0].ShiftEnd, Is.EqualTo(base_.AddHours(8)));
        });
    }

    /// <summary>
    ///     Два StartShift и два StopShift — две закрытые смены.
    /// </summary>
    [Test]
    public void Build_TwoStartsTwoStops_TwoClosedShifts()
    {
        var base_ = new DateTimeOffset(2025, 3, 10, 9, 0, 0, TimeSpan.Zero);
        var transactions = new[]
        {
            MakeTransaction(TransactionType.StartShift, _employeeA, base_),
            MakeTransaction(TransactionType.StartShift, _employeeA, base_.AddHours(1)),
            MakeTransaction(TransactionType.StopShift, _employeeA, base_.AddHours(8)),
            MakeTransaction(TransactionType.StopShift, _employeeA, base_.AddHours(9))
        };

        var report = WorkScheduleReportBuilder.Build(transactions).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(report, Has.Count.EqualTo(2));
            Assert.That(report.All(r => r.ShiftEnd != null), Is.True);
            Assert.That(report[0].ShiftStart, Is.EqualTo(base_));
            Assert.That(report[0].ShiftEnd, Is.EqualTo(base_.AddHours(8)));
            Assert.That(report[1].ShiftStart, Is.EqualTo(base_.AddHours(1)));
            Assert.That(report[1].ShiftEnd, Is.EqualTo(base_.AddHours(9)));
        });
    }

    private Transaction MakeTransaction(
        TransactionType type,
        Employee employee,
        DateTimeOffset date)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Type = type,
            Amount = 0.01m,
            Discount = 0m,
            Quantity = 0.01m,
            TransactionDate = date,
            Owner = _org,
            Employee = employee,
            Nomenclature = _nomenclature
        };
    }
}