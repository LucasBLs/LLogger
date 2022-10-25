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
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentNullException("Invalid parameters for add the project name.");

            ProjectName = projectName;
        }

        [BsonId]
        public Guid? Id { get; set; } = default!;
        public string? ProjectName { get; set; } = default!;
        public DateTime? Date { get; set; } = DateTime.Now;
        public List<Job> Logs { get; set; } = new List<Job>();

        public void LogFunction(string jobName, string environment, string uniqueId, string description)
        {
            if (string.IsNullOrEmpty(jobName) ||
                string.IsNullOrEmpty(environment) ||
                string.IsNullOrEmpty(uniqueId) ||
                string.IsNullOrEmpty(description))
                throw new ArgumentNullException("Invalid parameters for call the LogFunction.");

            var _project = new Job
            {
                LogName = jobName,
                Environment = environment,
                UniqueId = uniqueId,
                Description = description
            };
            Logs?.Add(_project);
        }

        public void LogContent(string message, object content)
        {
            if (Logs.Any())
            {
                foreach (var item in Logs)
                {
                    item.LogsContent?.Add(new LogContent(
                        message,
                        content
                    ));
                    item.Success = true;
                }
            }
        }

        public void LogError(Exception exception, string? message = null)
        {
            if (Logs.Any())
            {
                foreach (var item in Logs)
                {
                    item.LogsContent?.Add(new LogContent(
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
                    query = query.Where(x => x.Logs.Any(i => i.LogName == job));

                if (success != null)
                    query = query.Where(x => x.Logs.Any(i => i.Success == success));

                if (!string.IsNullOrEmpty(uniqueId))
                    query = query.Where(x => x.Logs.Any(i => i.UniqueId == uniqueId));

                if (!string.IsNullOrEmpty(uniqueId))
                    query = query.Where(x => x.Logs.Any(i => i.LogsContent.Any(c => c.Message == message)));

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
        public string? LogName { get; set; } = default!;
        public string? Environment { get; set; } = default!;
        public string? UniqueId { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool Success { get; set; } = default!;
        public List<LogContent>? LogsContent { get; set; } = new List<LogContent>();
    }

    public class LogContent
    {
        public LogContent(string? message, object? content)
        {
            Message = message;
            Content = content;
            CreatedAt = DateTime.Now;
        }

        public string? Message { get; set; } = default!;
        public object? Content { get; set; } = default!;
        public DateTime? CreatedAt { get; set; } = default!;
    }
}