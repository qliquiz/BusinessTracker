using BusinessTracker.Domain;

namespace BusinessTracker.Tests;

public class TestPutin
{
    [Test]
    public void Check_Putin()
    {
        var class1 = new Putin();

        var result = class1.GetPutin();

        Assert.That(!string.IsNullOrWhiteSpace(result));
    }

    [Test]
    public void FailCheck_Putin()
    {
        // Assert.Fail("Test failed!");
    }

    [Test]
    public void PassCheck_Putin()
    {
        Assert.Pass("Test passed!");
    }
}