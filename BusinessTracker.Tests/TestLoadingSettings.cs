using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BusinessTracker.Data.Extensions;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
/// Интеграционные тесты для <see cref="ILoadingSettingsRepository"/>.
/// Требуют запущенной БД (docker-compose up).
/// </summary>
public class TestLoadingSettings
{
    private static readonly Guid SeedOrgId = new("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");

    private ServiceProvider _provider = null!;
    private Organization _org = null!;

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
    public void OneTimeTearDown() => _provider.Dispose();

    [SetUp]
    public void SetUp()
    {
        _org = new Organization
        {
            Id      = SeedOrgId,
            Name    = "Главный офис (Спб)",
            Inn     = "1234567890",
            Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
        };
    }

    private ILoadingSettingsRepository GetRepo()
    {
        var scope = _provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ILoadingSettingsRepository>();
    }

    /// <summary>
    /// Load не бросает исключение и возвращает объект (дефолтный или сохранённый).
    /// </summary>
    [Test]
    public void Load_LoadingSettingsRepository_NotThrowsException()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var result = await GetRepo().Load(_org, CancellationToken.None);
            Assert.That(result, Is.Not.Null);
        });
    }

    /// <summary>
    /// Save сохраняет настройки, Load возвращает те же значения.
    /// </summary>
    [Test]
    public async Task Save_Then_Load_ReturnsPersistedValues()
    {
        var settings = new LoadingSettings
        {
            Owner         = _org,
            Description   = "Integration test settings",
            StartPosition = 42,
            BatchSize     = 250
        };

        await GetRepo().Save(settings, CancellationToken.None);
        var loaded = await GetRepo().Load(_org, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Description, Is.EqualTo(settings.Description));
            Assert.That(loaded.StartPosition, Is.EqualTo(settings.StartPosition));
            Assert.That(loaded.BatchSize, Is.EqualTo(settings.BatchSize));
        });
    }

    /// <summary>
    /// Повторный Save перезаписывает настройки — Load возвращает последние значения.
    /// </summary>
    [Test]
    public async Task Save_Twice_Load_ReturnsLastValues()
    {
        var first = new LoadingSettings
        {
            Owner = _org, Description = "First", StartPosition = 1, BatchSize = 100
        };
        var second = new LoadingSettings
        {
            Owner = _org, Description = "Second", StartPosition = 99, BatchSize = 500
        };

        await GetRepo().Save(first, CancellationToken.None);
        await GetRepo().Save(second, CancellationToken.None);
        var loaded = await GetRepo().Load(_org, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(loaded.Description, Is.EqualTo(second.Description));
            Assert.That(loaded.StartPosition, Is.EqualTo(second.StartPosition));
            Assert.That(loaded.BatchSize, Is.EqualTo(second.BatchSize));
        });
    }

    /// <summary>
    /// Load для несуществующей организации бросает <see cref="InvalidDataException"/>.
    /// </summary>
    [Test]
    public void Load_UnknownOrganization_ThrowsInvalidDataException()
    {
        var unknownOrg = new Organization
        {
            Id      = Guid.NewGuid(),
            Name    = "Unknown",
            Inn     = "0000000000",
            Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
        };

        Assert.ThrowsAsync<InvalidDataException>(async () =>
            await GetRepo().Load(unknownOrg, CancellationToken.None));
    }

    /// <summary>
    /// Save для несуществующей организации бросает <see cref="InvalidDataException"/>.
    /// </summary>
    [Test]
    public void Save_UnknownOrganization_ThrowsInvalidDataException()
    {
        var unknownOrg = new Organization
        {
            Id      = Guid.NewGuid(),
            Name    = "Unknown",
            Inn     = "0000000000",
            Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
        };

        var settings = new LoadingSettings
        {
            Owner = unknownOrg, Description = "test", StartPosition = 0, BatchSize = 100
        };

        Assert.ThrowsAsync<InvalidDataException>(async () =>
            await GetRepo().Save(settings, CancellationToken.None));
    }
}
