using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Lemos.Logger
{
    public class LLogger
    {
        public LLogger(string? projectName)
        {
            ProjectName = projectName;
            Functions = new List<Function>();
        }

        [BsonId]
        public Guid? Id { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public List<Function> Functions { get; set; }

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

        public async static Task SaveLogsAsync(LLogger log)
        {
            try
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                log.Id = Guid.NewGuid();
                var collection = await LLConnection.GetCollectionAsync();
                await collection.InsertOneAsync(log);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async static Task<List<LLogger>> SearchLogsAsync(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10)
        {
            try
            {
                var collection = await LLConnection.GetCollectionAsync();
                var filter = Builders<LLogger>.Filter.Where(x => x.Date >= startDate && x.Date <= endDate);
                return await collection.Find(filter).Skip((page - 1) * pageSize).Limit(pageSize).SortByDescending(a => a.Date).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class Function
    {
        public string? JobName { get; set; }
        public string? Environment { get; set; }
        public string? UniqueId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string? Result { get; set; }
        public List<Log>? Logs { get; set; } = new List<Log>();
    }

    public class Log
    {
        public Log(string? functionName, object? content)
        {
            FunctionName = functionName;
            Content = content;
            CreatedAt = DateTime.Now;
        }

        public string? FunctionName { get; set; }
        public object? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}