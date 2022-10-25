using Lemos.Logger;

namespace Lemos.Tests;

[TestClass]
public class LLConnectionTest
{
    [TestMethod]
    public async Task ConfigureDatabasePassStringEmptyInConnectionString()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            async () => await LLConnection.ConfigureDatabaseAsync("", "LLogger"));
    }

    [TestMethod]
    public async Task ConfigureDatabasePassStringEmptyInCollectionName()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            async () => await LLConnection.ConfigureDatabaseAsync("LLogger", ""));
    }
}