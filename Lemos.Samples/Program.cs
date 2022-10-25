using Lemos.Logger;
using Newtonsoft.Json;

#region  ConfigureDatabase
await LLConnection.ConfigureDatabaseAsync("mongodb://localhost:27017", "Sample");
#endregion

#region Logging
var logger = new LLogger("LLoger");
try
{
    logger.LogFunction("Test", "Dev", "d80f98c6-e126-46da-85b9-9273ce92a098", "Write in console text");
    var text = "Hello, World!";
    logger.LogContent("Writing text", text);
    logger.LogContent("Writing text2", text);
    Convert.ToDateTime(text);
    Console.WriteLine(text);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error:");
    throw;
}
finally
{
    await LLogger.SaveLogsAsync(logger);
}
#endregion

#region SearchLogs
var logs = await LLogger.SearchLogsAsync(DateTime.Now.AddHours(-20), DateTime.Now, projectName: "LLoger");
await File.WriteAllTextAsync(
    "Sample.json", 
    JsonConvert.SerializeObject(logs, Formatting.Indented));
#endregion