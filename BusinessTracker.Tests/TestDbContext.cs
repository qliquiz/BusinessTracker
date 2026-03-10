using BusinessTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Tests;

/// <summary>
/// Набор тестов для проверки работы контекста базы данных.
/// </summary>
public class TestDbContext
{
    /// <summary>
    /// Проверить выборку данных. Получить список всех организаций.
    /// </summary>
    [Test]
    public async Task FetchCompanies_Any()
    {
        // Подготовка
        var ctx = new BusinessTrackerContext();

        // Действие
        var result = await ctx.Organizations.ToListAsync(CancellationToken.None);

        // Проверка
        Assert.That(result, Is.Not.Empty);
    }
}