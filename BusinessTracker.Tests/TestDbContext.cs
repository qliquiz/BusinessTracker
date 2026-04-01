using BusinessTracker.Data;
using BusinessTracker.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace BusinessTracker.Tests;

/// <summary>
/// Набор тестов для проверки работы контекста базы данных.
/// </summary>
public class TestDbContext
{
    private ServiceProvider _provider = null!;

    [OneTimeSetUp]
    public void Setup()
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
    public void TearDown() => _provider.Dispose();

    /// <summary>
    /// Проверить выборку данных. Получить список всех организаций.
    /// </summary>
    [Test]
    public async Task FetchCompanies_Any()
    {
        using var scope = _provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<BusinessTrackerContext>();

        var result = await ctx.Organizations.ToListAsync(CancellationToken.None);

        Assert.That(result, Is.Not.Empty);
    }
}
