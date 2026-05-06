using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Data.Extensions;
using BusinessTracker.Data.Models;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessTracker.Tests;

/// <summary>
///     Функциональные тесты для <see cref="IEntityExtractor" />.
///     Требуют запущенной БД (docker-compose up).
/// </summary>
public class TestEntityExtractor
{
    private static readonly Guid OrgId = new("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");

    private ServiceProvider _provider = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        _provider = new ServiceCollection()
            .RegisterBusinessTrackerData(configuration)
            .BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _provider.Dispose();
    }

    /// <summary>
    ///     Новая категория из строк журнала возвращается, если её нет в БД.
    /// </summary>
    [Test]
    public async Task ExtractNewCategories_NewName_ReturnsCategory()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();

        var uniqueName = $"TestCat_{Guid.NewGuid():N}";
        var rows = new List<JournalRowDto>
        {
            BuildRow(uniqueName, "Товар1", "Иван")
        };

        var result = (await extractor.ExtractNewCategoriesAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo(uniqueName));
    }

    /// <summary>
    ///     Уже существующая категория не возвращается повторно.
    /// </summary>
    [Test]
    public async Task ExtractNewCategories_ExistingName_ReturnsEmpty()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();
        var ctx = scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>();

        var existingName = $"ExistCat_{Guid.NewGuid():N}";
        ctx.Categories.Add(new Category { Id = Guid.NewGuid(), Name = existingName, OwnerId = OrgId });
        await ctx.SaveChangesAsync();

        var rows = new List<JournalRowDto>
        {
            BuildRow(existingName, "Товар2", "Иван")
        };

        var result = (await extractor.ExtractNewCategoriesAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    ///     Дубликаты категорий в строках журнала не приводят к дублям в результате.
    /// </summary>
    [Test]
    public async Task ExtractNewCategories_DuplicateRows_ReturnsDistinct()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();

        var uniqueName = $"DupCat_{Guid.NewGuid():N}";
        var rows = new List<JournalRowDto>
        {
            BuildRow(uniqueName, "Т1", "А"),
            BuildRow(uniqueName, "Т2", "Б")
        };

        var result = (await extractor.ExtractNewCategoriesAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
    }

    /// <summary>
    ///     Новая номенклатура возвращается, если её нет в БД, и для неё есть категория.
    /// </summary>
    [Test]
    public async Task ExtractNewNomenclature_NewName_ReturnsNomenclature()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();
        var ctx = scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>();

        var catName = $"NomCat_{Guid.NewGuid():N}";
        var nomName = $"TestNom_{Guid.NewGuid():N}";
        ctx.Categories.Add(new Category { Id = Guid.NewGuid(), Name = catName, OwnerId = OrgId });
        await ctx.SaveChangesAsync();

        var rows = new List<JournalRowDto>
        {
            BuildRow(catName, nomName, "Иван")
        };

        var result = (await extractor.ExtractNewNomenclatureAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo(nomName));
    }

    /// <summary>
    ///     Номенклатура без соответствующей категории в БД не возвращается.
    /// </summary>
    [Test]
    public async Task ExtractNewNomenclature_MissingCategory_ReturnsEmpty()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();

        var rows = new List<JournalRowDto>
        {
            BuildRow(
                $"NoSuchCat_{Guid.NewGuid():N}",
                $"Nom_{Guid.NewGuid():N}",
                "Иван")
        };

        var result = (await extractor.ExtractNewNomenclatureAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    ///     Новый сотрудник из строк журнала возвращается, если его нет в БД.
    /// </summary>
    [Test]
    public async Task ExtractNewEmployees_NewName_ReturnsEmployee()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();

        var uniqueName = $"Emp_{Guid.NewGuid():N}";
        var rows = new List<JournalRowDto>
        {
            BuildRow("Кат", "Ном", uniqueName)
        };

        var result = (await extractor.ExtractNewEmployeesAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo(uniqueName));
    }

    /// <summary>
    ///     Уже существующий сотрудник не возвращается повторно.
    /// </summary>
    [Test]
    public async Task ExtractNewEmployees_ExistingName_ReturnsEmpty()
    {
        using var scope = _provider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();
        var ctx = scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>();

        var existingName = $"ExistEmp_{Guid.NewGuid():N}";
        ctx.Employees.Add(new Employee
        {
            Id = Guid.NewGuid(), Name = existingName, OwnerId = OrgId, Role = 0
        });
        await ctx.SaveChangesAsync();

        var rows = new List<JournalRowDto>
        {
            BuildRow("Кат", "Ном", existingName)
        };

        var result = (await extractor.ExtractNewEmployeesAsync(OrgId, rows, CancellationToken.None)).ToList();

        Assert.That(result, Is.Empty);
    }

    private static JournalRowDto BuildRow(string categoryName, string nomenclatureName, string employeeName)
    {
        return new JournalRowDto
        {
            Code = new Random().Next(1_000_000, 9_999_999),
            TypeCode = 1,
            TransTypeName = "Sale",
            ReceiptNumber = 1,
            Period = DateTime.UtcNow,
            Quantity = 1,
            Price = 100,
            Discount = 0,
            CategoryName = categoryName,
            NomenclatureName = nomenclatureName,
            EmployeeName = employeeName
        };
    }
}