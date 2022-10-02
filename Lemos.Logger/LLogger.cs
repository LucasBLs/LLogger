using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
            Jobs = new List<Job>();
        }

        [BsonId]
        public Guid? Id { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public List<Job> Jobs { get; set; }

        public void LogFunction(string jobName, string environment, string uniqueId, string description)
        {
            var _project = new Job
            {
                JobName = jobName,
                Environment = environment,
                UniqueId = uniqueId,
                Description = description
            };
            Jobs?.Add(_project);
        }

        public void LogContent(string message, object content)
        {
            if (Jobs.Any())
            {
                foreach (var item in Jobs)
                {
                    item.Logs?.Add(new Log(
                        message,
                        content
                    ));
                    item.Success = true;
                }
            }
        }

        public void LogError(Exception exception, string? message = null)
        {
            if (Jobs.Any())
            {
                foreach (var item in Jobs)
                {
                    item.Logs?.Add(new Log(
                        string.IsNullOrEmpty(message) is true ? exception.TargetSite.ToString() : message,
                        exception.ToString()
                    ));
                    item.Success = false;
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

        public async static Task<List<LLogger>> SearchLogsAsync(DateTime startDate, DateTime endDate, int skip = 0, int take = 25, string? projectName = null, string? job = null, string? uniqueId = null, string? message = null, bool? success = null)
        {
            try
            {
                var collection = await LLConnection.GetCollectionAsync();
                var query = collection.AsQueryable().Where(x => x.Date >= startDate && x.Date <= endDate);

                if (!string.IsNullOrEmpty(projectName))
                    query = query.Where(x => x.ProjectName == projectName);

                if (!string.IsNullOrEmpty(job))
                    query = query.Where(x => x.Jobs.Any(i => i.JobName == job));

                if (success != null)
                    query = query.Where(x => x.Jobs.Any(i => i.Success == success));

                if (!string.IsNullOrEmpty(uniqueId))
                    query = query.Where(x => x.Jobs.Any(i => i.UniqueId == uniqueId));

                if (!string.IsNullOrEmpty(uniqueId))
                    query = query.Where(x => x.Jobs.Any(i => i.Logs.Any(c => c.Message == message)));

                return query.Skip(skip).Take(take).OrderByDescending(x => x.Date).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class Job
    {
        public string? JobName { get; set; }
        public string? Environment { get; set; }
        public string? UniqueId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool Success { get; set; }
        public List<Log>? Logs { get; set; } = new List<Log>();
    }

    public class Log
    {
        public Log(string? message, object? content)
        {
            Message = message;
            Content = content;
            CreatedAt = DateTime.Now;
        }

        public string? Message { get; set; }
        public object? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}