using System.Diagnostics;
using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
/// Сравнительные замеры производительности оригинальных (LINQ) и оптимизированных (Dictionary)
/// построителей отчётов на синтетических данных.
/// </summary>
/// <remarks>
/// Запуск только бенчмарков:
/// <code>dotnet test --filter "FullyQualifiedName~BenchmarkReportBuilders" -- NUnit.DefaultTimeout=120000</code>
/// Подробный вывод с таймингами:
/// <code>dotnet test --filter "FullyQualifiedName~BenchmarkReportBuilders" --logger "console;verbosity=detailed"</code>
/// </remarks>
[TestFixture]
public class BenchmarkReportBuilders
{
    private const string KladrAddress =
        "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12";

    private static readonly Organization[] Orgs =
    [
        new() { Id = Guid.NewGuid(), Name = "Офис СПБ", Inn = "1234567890", Address = KladrAddress },
        new() { Id = Guid.NewGuid(), Name = "Офис ИРК", Inn = "0987654321", Address = KladrAddress }
    ];

    // Небольшой фиксированный набор для JIT-разогрева
    private static readonly List<Transaction> WarmupData = GenerateTransactions(50);

    // ──────────────────────────────────────────────────────────────────────────
    //  Разогрев
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Выполняется один раз перед всеми тестами класса.
    /// Прогревает JIT для всех методов обоих вариантов построителей,
    /// чтобы последующие замеры не включали накладные расходы первичной компиляции.
    /// </summary>
    [OneTimeSetUp]
    public void WarmUp()
    {
        _ = RevenueReportBuilder.Build(WarmupData).ToList();
        _ = RevenueReportBuilder.BuildOptimized(WarmupData).ToList();
        _ = SalesReportBuilder.Build(WarmupData).ToList();
        _ = SalesReportBuilder.BuildOptimized(WarmupData).ToList();
        _ = WorkScheduleReportBuilder.Build(WarmupData).ToList();
        _ = WorkScheduleReportBuilder.BuildOptimized(WarmupData).ToList();
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  Отчёт "Выручка"
    // ──────────────────────────────────────────────────────────────────────────

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_RevenueReport_Linq(int receiptCount)
    {
        var transactions = GenerateTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = RevenueReportBuilder.Build(transactions).ToList();
        sw.Stop();

        PrintResult("RevenueReport   LINQ", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_RevenueReport_Optimized(int receiptCount)
    {
        var transactions = GenerateTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = RevenueReportBuilder.BuildOptimized(transactions).ToList();
        sw.Stop();

        PrintResult("RevenueReport   Dict", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  Отчёт "Продажи"
    // ──────────────────────────────────────────────────────────────────────────

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_SalesReport_Linq(int receiptCount)
    {
        var transactions = GenerateTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = SalesReportBuilder.Build(transactions).ToList();
        sw.Stop();

        PrintResult("SalesReport     LINQ", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_SalesReport_Optimized(int receiptCount)
    {
        var transactions = GenerateTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = SalesReportBuilder.BuildOptimized(transactions).ToList();
        sw.Stop();

        PrintResult("SalesReport     Dict", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  Отчёт "График работы"
    // ──────────────────────────────────────────────────────────────────────────

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_WorkScheduleReport_Linq(int receiptCount)
    {
        var transactions = GenerateTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = WorkScheduleReportBuilder.Build(transactions).ToList();
        sw.Stop();

        PrintResult("WorkSchedule    LINQ", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_WorkScheduleReport_Optimized(int receiptCount)
    {
        var transactions = GenerateTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = WorkScheduleReportBuilder.BuildOptimized(transactions).ToList();
        sw.Stop();

        PrintResult("WorkSchedule    Dict", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  Сценарий смешанной оплаты
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Проверяет оба построителя на данных со смешанной оплатой.
    /// Смешанная оплата — чек, сумма которого разбита на наличную и безналичную части;
    /// каждая часть представлена отдельной Sale-транзакцией.
    /// Поле PaymentType будет добавлено при интеграции MSSQL-данных;
    /// пока обе транзакции неразличимы по типу оплаты.
    /// </summary>
    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_RevenueReport_MixedPayment_Linq(int receiptCount)
    {
        var transactions = GenerateMixedPaymentTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = RevenueReportBuilder.Build(transactions).ToList();
        sw.Stop();

        PrintResult("Revenue MixPay  LINQ", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(100_000)]
    public void Benchmark_RevenueReport_MixedPayment_Optimized(int receiptCount)
    {
        var transactions = GenerateMixedPaymentTransactions(receiptCount);
        var sw = Stopwatch.StartNew();
        var rows = RevenueReportBuilder.BuildOptimized(transactions).ToList();
        sw.Stop();

        PrintResult("Revenue MixPay  Dict", receiptCount, transactions.Count, rows.Count, sw.Elapsed);
        Assert.That(rows, Is.Not.Empty);
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  Генераторы синтетических данных
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Генерирует реалистичный набор транзакций для <paramref name="receiptCount"/> чеков.
    /// </summary>
    /// <remarks>
    /// Правила генерации:
    /// <list type="bullet">
    ///   <item>Чек = 1–4 позиции Sale; Amount = цена × кол-во − скидка.</item>
    ///   <item>~5 % позиций имеют скидку 100 % (Amount приведён к минимуму 0.01).</item>
    ///   <item>~30 % чеков — наличный расчёт со сдачей (Change).</item>
    ///   <item>~10 % чеков содержат возврат (Return) отдельной позиции.</item>
    ///   <item>Каждый сотрудник получает по одной паре StartShift/StopShift на каждые 200 чеков.</item>
    /// </list>
    /// Генератор детерминирован: фиксированный <paramref name="seed"/> даёт одни и те же данные.
    /// </remarks>
    private static List<Transaction> GenerateTransactions(int receiptCount, int seed = 42)
    {
        var rng = new Random(seed);
        var result = new List<Transaction>(receiptCount * 4);

        var (nomenclatures, employees) = BuildDictionaries();

        AddShifts(result, employees, nomenclatures[0], receiptCount, rng);

        var baseDate = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero);

        for (var receipt = 0; receipt < receiptCount; receipt++)
        {
            var org = Orgs[rng.Next(Orgs.Length)];
            var emp = employees[rng.Next(employees.Length)];
            var date = baseDate.AddDays(rng.Next(365)).AddHours(rng.Next(10, 20)).AddMinutes(rng.Next(60));

            // 1–4 позиции в чеке
            var lineCount = rng.Next(1, 5);
            for (var line = 0; line < lineCount; line++)
            {
                result.Add(MakeSale(rng, org, emp, nomenclatures, date));
            }

            // ~30 % — наличный расчёт со сдачей
            if (rng.Next(100) < 30)
                result.Add(MakeChange(rng, org, emp, nomenclatures, date));

            // ~10 % — возврат
            if (rng.Next(100) < 10)
                result.Add(MakeReturn(rng, org, emp, nomenclatures, date));
        }

        return result;
    }

    /// <summary>
    /// Генерирует чеки со смешанной оплатой.
    /// </summary>
    /// <remarks>
    /// Каждый чек содержит 1–3 товарные позиции (Sale).
    /// Затем сумма чека делится на наличную и безналичную части
    /// (две отдельные Sale-транзакции с разбивкой ~60/40).
    /// Наличная часть сопровождается транзакцией Change (сдача).
    /// ~5 % позиций — скидка 100 %.
    /// ~10 % чеков — возврат одной позиции.
    /// </remarks>
    private static List<Transaction> GenerateMixedPaymentTransactions(int receiptCount, int seed = 42)
    {
        var rng = new Random(seed);
        // Смешанная оплата даёт ~4–5 транзакций на чек
        var result = new List<Transaction>(receiptCount * 5);

        var (nomenclatures, employees) = BuildDictionaries();

        AddShifts(result, employees, nomenclatures[0], receiptCount, rng);

        var baseDate = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero);

        for (var receipt = 0; receipt < receiptCount; receipt++)
        {
            var org = Orgs[rng.Next(Orgs.Length)];
            var emp = employees[rng.Next(employees.Length)];
            var date = baseDate.AddDays(rng.Next(365)).AddHours(rng.Next(10, 20)).AddMinutes(rng.Next(60));

            // 1–3 товарные позиции
            var lineCount = rng.Next(1, 4);
            var totalAmount = 0m;
            for (var line = 0; line < lineCount; line++)
            {
                var sale = MakeSale(rng, org, emp, nomenclatures, date);
                result.Add(sale);
                totalAmount += sale.Amount;
            }

            // Смешанная оплата: ~60 % наличными, ~40 % безналично
            // (PaymentType будет добавлен позже; пока — две отдельные Sale-транзакции)
            var cashPart = Math.Round(totalAmount * 0.6m, 2);
            var cardPart = Math.Max(0.01m, totalAmount - cashPart);

            result.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Sale,
                Amount = cashPart,
                Discount = 0,
                Quantity = 1,
                TransactionDate = date,
                Owner = org,
                Nomenclature = nomenclatures[rng.Next(nomenclatures.Length)],
                Employee = emp
            });

            result.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Sale,
                Amount = cardPart,
                Discount = 0,
                Quantity = 1,
                TransactionDate = date,
                Owner = org,
                Nomenclature = nomenclatures[rng.Next(nomenclatures.Length)],
                Employee = emp
            });

            // Сдача с наличной части
            var change = Math.Round((decimal)(rng.NextDouble() * 50 + 1), 2);
            result.Add(MakeChange(rng, org, emp, nomenclatures, date, change));

            // ~10 % — возврат
            if (rng.Next(100) < 10)
                result.Add(MakeReturn(rng, org, emp, nomenclatures, date));
        }

        return result;
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  Вспомогательные методы
    // ──────────────────────────────────────────────────────────────────────────

    private static (Nomenclature[] nomenclatures, Employee[] employees)
        BuildDictionaries()
    {
        var categories = Enumerable.Range(1, 5)
            .Select(i => new Category { Id = Guid.NewGuid(), Name = $"Категория {i}", Owner = Orgs[i % Orgs.Length] })
            .ToArray();

        var nomenclatures = Enumerable.Range(1, 20)
            .Select(i => new Nomenclature
                { Id = Guid.NewGuid(), Name = $"Товар {i}", Category = categories[i % categories.Length] })
            .ToArray();

        var employees = Enumerable.Range(1, 8)
            .Select(i => new Employee { Id = Guid.NewGuid(), Name = $"Сотрудник {i}", Owner = Orgs[i % Orgs.Length] })
            .ToArray();

        return (nomenclatures, employees);
    }

    private static void AddShifts(
        List<Transaction> result, Employee[] employees, Nomenclature nom, int receiptCount, Random rng)
    {
        var shiftsPerEmployee = Math.Max(1, receiptCount / 200);
        var baseDate = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero);

        foreach (var emp in employees)
        {
            for (var s = 0; s < shiftsPerEmployee; s++)
            {
                var shiftStart = baseDate.AddDays(s).AddMinutes(rng.Next(0, 30));
                result.Add(MakeShift(TransactionType.StartShift, shiftStart, emp, nom));
                result.Add(
                    MakeShift(TransactionType.StopShift, shiftStart.AddHours(8 + rng.NextDouble() * 4), emp, nom));
            }
        }
    }

    private static Transaction MakeSale(
        Random rng, Organization org, Employee emp, Nomenclature[] noms, DateTimeOffset date)
    {
        var nom = noms[rng.Next(noms.Length)];
        var unitPrice = Math.Round((decimal)(rng.NextDouble() * 990 + 10), 2);
        var qty = Math.Round((decimal)(rng.NextDouble() * 4 + 1), 2);
        var grossAmount = unitPrice * qty;

        bool fullDiscount = rng.Next(100) < 5;
        var discount = fullDiscount
            ? grossAmount
            : Math.Round((decimal)rng.NextDouble() * 0.4m * grossAmount, 2);

        return new Transaction
        {
            Id = Guid.NewGuid(),
            Type = TransactionType.Sale,
            Amount = Math.Max(0.01m, grossAmount - discount),
            Discount = discount,
            Quantity = qty,
            TransactionDate = date,
            Owner = org,
            Nomenclature = nom,
            Employee = emp
        };
    }

    private static Transaction MakeChange(
        Random rng, Organization org, Employee emp, Nomenclature[] noms, DateTimeOffset date,
        decimal? fixedAmount = null)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Type = TransactionType.Change,
            Amount = fixedAmount ?? Math.Round((decimal)(rng.NextDouble() * 100 + 1), 2),
            Discount = 0,
            Quantity = 1,
            TransactionDate = date,
            Owner = org,
            Nomenclature = noms[rng.Next(noms.Length)],
            Employee = emp
        };
    }

    private static Transaction MakeReturn(
        Random rng, Organization org, Employee emp, Nomenclature[] noms, DateTimeOffset date)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Type = TransactionType.Return,
            Amount = Math.Round((decimal)(rng.NextDouble() * 200 + 5), 2),
            Discount = 0,
            Quantity = 1,
            TransactionDate = date,
            Owner = org,
            Nomenclature = noms[rng.Next(noms.Length)],
            Employee = emp
        };
    }

    private static Transaction MakeShift(
        TransactionType type, DateTimeOffset date, Employee emp, Nomenclature nom) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Amount = 0.01m,
            Discount = 0,
            Quantity = 1,
            TransactionDate = date,
            Owner = emp.Owner,
            Nomenclature = nom,
            Employee = emp
        };

    private static void PrintResult(string name, int receiptCount, int txCount, int rowCount, TimeSpan elapsed)
    {
        TestContext.Out.WriteLine(
            $"[{name}] чеков={receiptCount,7} | транзакций={txCount,9} | строк={rowCount,6} | {elapsed.TotalMilliseconds,8:F2} мс");
    }
}