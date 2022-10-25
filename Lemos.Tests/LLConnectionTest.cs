using Lemos.Logger;

namespace Lemos.Tests;

[TestClass]
public class LLConnectionTest
{
    [TestMethod]
    public async Task ConfigureDatabasePassNullInConnectionString()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            async () => await LLConnection.ConfigureDatabaseAsync("","LLogger"));
    }

    [TestMethod]
    public async Task ConfigureDatabasePassNullInCollectionName()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            async () => await LLConnection.ConfigureDatabaseAsync("LLogger",""));
    }
}