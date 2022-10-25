using Lemos.Logger;
using Newtonsoft.Json;

#region  ConfigureDatabase
await LLConnection.ConfigureDatabaseAsync("mongodb://localhost:27017", "Sample");
#endregion

#region Logging
var logger = new LLogger("");
try
{
    logger.LogFunction("Test", "Dev", "d80f98c6-e126-46da-85b9-9273ce92a098", "Write in console text");
    var text = "Hello, World!";
    logger.LogContent("Writing text", text);
    logger.LogContent("Writing text2", text);
    Console.WriteLine(text);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error: ");
    throw;
}
finally
{
    await LLogger.SaveLogsAsync(logger);
}
#endregion

#region SearchLogs
var logs = await LLogger.SearchLogsAsync(DateTime.Now.AddHours(-2), DateTime.Now, projectName: "SampleTest");
var logsIndented = JsonConvert.SerializeObject(logs, Formatting.Indented);
Console.WriteLine(logsIndented);
#endregion