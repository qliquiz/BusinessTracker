using BusinessTracker.Data;
using BusinessTracker.Data.Extensions;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BusinessTracker.Tests;

/// <summary>
///     Интеграционные тесты для сервиса загрузки транзакций.
///     Требуют запущенной БД (docker-compose up).
/// </summary>
public class TestLoadingService
{
    private static readonly Guid SeedOrgId = new("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
    private Organization _org = null!;

    private ServiceProvider _provider = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // Гарантируем наличие таблицы JournalRows перед тестами
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                              CREATE TABLE IF NOT EXISTS "JournalRows"
                              (
                                  "Id"               UUID             NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
                                  "OrganizationId"   UUID             NOT NULL REFERENCES "Organizations" ("Id"),
                                  "Code"             BIGINT           NOT NULL,
                                  "TypeCode"         INT              NOT NULL,
                                  "TransTypeName"    TEXT             NOT NULL DEFAULT '',
                                  "ReceiptNumber"    INT              NOT NULL DEFAULT 0,
                                  "ProductCode"      BIGINT,
                                  "CategoryCode"     BIGINT,
                                  "EmployeeCode"     INT,
                                  "Period"           TIMESTAMP        NOT NULL,
                                  "Quantity"         DOUBLE PRECISION NOT NULL DEFAULT 0,
                                  "Price"            DOUBLE PRECISION NOT NULL DEFAULT 0,
                                  "Discount"         DOUBLE PRECISION NOT NULL DEFAULT 0,
                                  "RawId"            INT              NOT NULL DEFAULT 0,
                                  "RawLoginId"       INT              NOT NULL DEFAULT 0,
                                  "EmployeeName"     TEXT             NOT NULL DEFAULT '',
                                  "CategoryName"     TEXT             NOT NULL DEFAULT '',
                                  "NomenclatureName" TEXT             NOT NULL DEFAULT ''
                              );
                              """;
            cmd.ExecuteNonQuery();
        }

        _provider = new ServiceCollection()
            .RegisterBusinessTrackerData(configuration)
            .BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _provider.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _org = new Organization
        {
            Id = SeedOrgId,
            Name = "Главный офис (Спб)",
            Inn = "1234567890",
            Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
        };

        // Сбрасываем LoadOptions перед каждым тестом, чтобы StartPosition не влиял на следующий
        using var scope = _provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>();
        var org = ctx.Organizations.First(o => o.Id == SeedOrgId);
        org.LoadOptions = null;
        ctx.SaveChanges();
    }

    private (ILoadingSettingsRepository settings, IJournalRowsRepository journal, BusinessTrackerContext ctx)
        GetServices()
    {
        var scope = _provider.CreateScope();
        return (
            scope.ServiceProvider.GetRequiredService<ILoadingSettingsRepository>(),
            scope.ServiceProvider.GetRequiredService<IJournalRowsRepository>(),
            scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>()
        );
    }

    /// <summary>
    ///     Push с пустым списком транзакций возвращает false.
    /// </summary>
    [Test]
    public async Task Push_EmptyTransactions_ReturnsFalse()
    {
        var (settingsRepo, journalRepo, _) = GetServices();
        var loadingService = new LoadingServiceTestAdapter(settingsRepo, journalRepo);

        var result = await loadingService.PushAsync(_org, [], CancellationToken.None);

        Assert.That(result, Is.False);
    }

    /// <summary>
    ///     Push с валидными транзакциями возвращает true.
    /// </summary>
    [Test]
    public async Task Push_ValidTransactions_ReturnsTrue()
    {
        var (settingsRepo, journalRepo, _) = GetServices();
        var loadingService = new LoadingServiceTestAdapter(settingsRepo, journalRepo);

        var transactions = BuildTransactions(9000001, 3);

        var result = await loadingService.PushAsync(_org, transactions, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    /// <summary>
    ///     Push сохраняет строки в таблицу JournalRows.
    /// </summary>
    [Test]
    public async Task Push_ValidTransactions_SavesJournalRows()
    {
        var (settingsRepo, journalRepo, ctx) = GetServices();
        var loadingService = new LoadingServiceTestAdapter(settingsRepo, journalRepo);

        var startCode = 9999001L + new Random().Next(0, 9000);
        var transactions = BuildTransactions(startCode, 2);

        await loadingService.PushAsync(_org, transactions, CancellationToken.None);

        var saved = await ctx.JournalRows
            .Where(r => r.OrganizationId == SeedOrgId && r.Code >= startCode)
            .ToListAsync(CancellationToken.None);

        Assert.That(saved, Has.Count.GreaterThanOrEqualTo(2));
    }

    // -------------------------------------------------------------------------

    private static List<JournalRowDto> BuildTransactions(long startCode, int count)
    {
        return Enumerable.Range(0, count).Select(i => new JournalRowDto
        {
            Code = startCode + i,
            TypeCode = 1,
            TransTypeName = "Sale",
            ReceiptNumber = 100 + i,
            Period = DateTime.UtcNow,
            Quantity = 1,
            Price = 100 + i,
            Discount = 0,
            EmployeeName = "Тест",
            CategoryName = "Категория",
            NomenclatureName = "Товар"
        }).ToList();
    }
}

/// <summary>
///     Адаптер для тестирования логики Push без зависимости от Api-проекта.
///     Дублирует логику <c>LoadingService</c> из BusinessTracker.Api.
/// </summary>
file class LoadingServiceTestAdapter
{
    private readonly IJournalRowsRepository _journal;
    private readonly ILoadingSettingsRepository _settings;

    public LoadingServiceTestAdapter(ILoadingSettingsRepository settings, IJournalRowsRepository journal)
    {
        _settings = settings;
        _journal = journal;
    }

    public async Task<bool> PushAsync(Organization organization, IEnumerable<JournalRowDto> transactions,
        CancellationToken token)
    {
        LoadingSettings settings;
        try
        {
            settings = await _settings.Load(organization, token);
        }
        catch
        {
            settings = new LoadingSettings
                { Owner = organization, Description = "Default", StartPosition = 0, BatchSize = 1000 };
        }

        var list = transactions.Where(x => x.Code >= settings.StartPosition).ToList();
        if (list.Count == 0) return false;

        settings.StartPosition = list.Max(x => x.Code) + 1;
        await _settings.Save(settings, token);
        await _journal.SaveAsync(organization.Id, list, token);
        return true;
    }
}