using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Lemos.Logger.Model
{
    public class LLoggerModel
    {
        public LLoggerModel(string? projectName)
        {
            ProjectName = projectName;
            Functions = new List<Function>();
        }

        [BsonId]
        public Guid? Id { get; set; }
        public string? ProjectName { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? Date { get; set; } = DateTime.Now;
        public List<Function> Functions { get; set; }

        public void LogProject(string name)
            => ProjectName = name;

        public void LogFunction(string jobName, string environment, string uniqueId, string description)
        {
            var _project = new Function
            {
                JobName = jobName,
                Environment = environment,
                UniqueId = uniqueId,
                Description = description
            };
            Functions?.Add(_project);
        }

        public void LogContent(string functionName, object content)
        {
            if (Functions.Any())
            {
                foreach (var item in Functions)
                {
                    item.Logs?.Add(new Log(
                        functionName,
                        content
                    ));
                }
            }
        }
    }

    public class Function
    {
        public string? JobName { get; set; }
        public string? Environment { get; set; }
        public string? UniqueId { get; set; }
        public string? Description { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string? Result { get; set; }
        public List<Log>? Logs { get; set; }
    }

    public class Log
    {
        public Log(string? functionName, object? content)
        {
            FunctionName = functionName;
            Content = content;
        }

        public string? FunctionName { get; set; }
        public object? Content { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreatedAt { get; set; }
    }
}