using BusinessTracker.Domain;

namespace BusinessTracker.Tests;

/// <summary>
/// Базовые тесты работоспособности <see cref="CurrentApplication"/>.
/// </summary>
public class TestCurrentApplication
{
    /// <summary>
    /// Версия сборки не пустая.
    /// </summary>
    [Test]
    public void Check_CurrentApp()
    {
        var version = CurrentApplication.CurrentVersion();

        Assert.That(!string.IsNullOrWhiteSpace(version));
    }

    /// <summary>
    /// Заглушка для отрицательного сценария (пока не реализован).
    /// </summary>
    [Test]
    public void FailCheck_CurrentApp()
    {
        // Assert.Fail("Test failed!");
    }

    /// <summary>
    /// Безусловно проходящий тест — smoke check.
    /// </summary>
    [Test]
    public void PassCheck_CurrentApp()
    {
        Assert.Pass("Test passed!");
    }
}