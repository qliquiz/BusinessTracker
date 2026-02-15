using BusinessTracker.Domain;

namespace BusinessTracker.Tests;

public class TestCurrentApplication
{
    [Test]
    public void Check_CurrentApp()
    {
        var version = CurrentApplication.CurrentVersion();

        Assert.That(!string.IsNullOrWhiteSpace(version));
    }

    [Test]
    public void FailCheck_CurrentApp()
    {
        // Assert.Fail("Test failed!");
    }

    [Test]
    public void PassCheck_CurrentApp()
    {
        Assert.Pass("Test passed!");
    }
}