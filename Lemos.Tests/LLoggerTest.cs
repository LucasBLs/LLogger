using Lemos.Logger;

namespace Lemos.Tests;

[TestClass]
public class LLoggerTest
{
    [TestMethod]
    public void PassStringEmptyInProjectName()
    {
        Assert.ThrowsException<ArgumentNullException>(
             () => new LLogger(""));
    }

    [TestMethod]
    public void PassNullInProjectName()
    {
        Assert.ThrowsException<ArgumentNullException>(
             () => new LLogger(null));
    }

    [TestMethod]
    public void PassStringEmptyInLogFunction()
    {
        var logger = new LLogger("LLogger");
        Assert.ThrowsException<ArgumentNullException>(
             () => logger.LogFunction("", "", "", ""));
    }
}