using BusinessTracker.Data.Logics;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
/// Набор тестов для проверки загрузки настроек.
/// </summary>
public class TestLoadingSettings
{
    [Test]
    public void Load_LoadingSettingsRepository_NotThrowsException()
    {
        // Подготовка
        var repo = new LoadingSettingsRepository();
        var org = new Organization
        {
            Id = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"),
            Name = "",
            Address = ""
        };

        // Проверка и действие
        Assert.DoesNotThrowAsync(async () =>
        {
            var result = await repo.Load(org, CancellationToken.None);
            Assert.That(result, Is.Not.Null);
        });
    }
}