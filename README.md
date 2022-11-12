# LLogger

LemosLogger is a package for creating step-by-step logs in MongoDB.

| Package |  Version | Downloads |
| ------- | ----- | ----- |
| `LemosLogger` | [![NuGet](https://img.shields.io/nuget/v/LemosLogger.svg)](https://nuget.org/packages/LemosLogger) | [![Nuget](https://img.shields.io/nuget/dt/LemosLogger.svg)](https://nuget.org/packages/LemosLogger) |

### Dependencies
.NET Standard 2.1

You can check supported frameworks here:

https://docs.microsoft.com/pt-br/dotnet/standard/net-standard

### Instalation
This package is available through Nuget Packages: https://www.nuget.org/packages/LemosLogger

**Nuget**
```
Install-Package LemosLogger
```

**.NET CLI**
```
dotnet add package LemosLogger
```

```csharp
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
    logger.LogContent("Writing text", text);

    logger.LogFunction("Test2", "Dev", "d80f97c6-e126-46da-85b9-927231392a098", "Write in console text2");
    var text2 = "Hello, World2!";
    logger.LogContent("Writing text2", text2);
    logger.LogContent("Writing text2", text2);
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
#endregion
```

## Log result
```json
[
  {
    "Id": "e1ef00a3-dcef-46cc-a35f-79b08caa588c",
    "ProjectName": "LLogger",
    "Date": "2022-10-25T20:29:30.669Z",
    "Logs": [
      {
        "LogName": "Test",
        "Environment": "Dev",
        "UniqueId": "d80f98c6-e126-46da-85b9-9273ce92a098",
        "Description": "Write in console text",
        "CreatedAt": "2022-10-25T20:29:30.671Z",
        "Success": true,
        "LogsContent": [
          {
            "Message": "Writing text",
            "Content": "Hello, World!",
            "CreatedAt": "2022-10-25T20:29:30.671Z"
          },
          {
            "Message": "Writing text2",
            "Content": "Hello, World!",
            "CreatedAt": "2022-10-25T20:29:30.671Z"
          }
        ]
      },
      {
        "LogName": "Test2",
        "Environment": "Dev",
        "UniqueId": "d80f98c6-e126-46da-85b9-927231392a098",
        "Description": "Write in console text",
        "CreatedAt": "2022-10-25T20:29:30.671Z",
        "Success": true,
        "LogsContent": [
          {
            "Message": "Writing text",
            "Content": "Hello, World!",
            "CreatedAt": "2022-10-25T20:29:30.671Z"
          },
          {
            "Message": "Writing text2",
            "Content": "Hello, World!",
            "CreatedAt": "2022-10-25T20:29:30.671Z"
          }
        ]
      }
    ]
  }
]
```
