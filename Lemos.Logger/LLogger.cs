using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Lemos.Logger
{
    public class LLogger
    {
        /// <summary>
        /// <param name="projectName">Atribuir um nome para o projeto</param>
        /// </summary>     
        public LLogger(string? projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentNullException("Invalid parameters for add the project name.");

            ProjectName = projectName;
        }

        [BsonId]
        public string? Id { get; set; } = default!;
        public string? ProjectName { get; set; } = default!;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? Date { get; set; } = DateTime.Now;
        public List<Log> Logs { get; set; } = new List<Log>();

        /// <summary>
        /// <para name="logName">Atribuir um titulo para o log</para>
        /// <para name="environment">Tipo de ambiente, se é homologação ou produção</para>
        /// <para name="uniqueId">Chave única do log</para>
        /// <para name="description">Descrição do log</para>
        /// </summary>  
        public void LogFunction(string logName, string environment, string uniqueId, string description)
        {
            if (string.IsNullOrEmpty(logName) ||
                string.IsNullOrEmpty(environment) ||
                string.IsNullOrEmpty(uniqueId) ||
                string.IsNullOrEmpty(description))
                throw new ArgumentNullException("Invalid parameters for call the LogFunction.");

            var project = new Log
            {
                LogName = logName,
                Environment = environment,
                UniqueId = uniqueId,
                Description = description
            };
            Logs?.Add(project);
        }

        /// <summary>
        /// <para name="message">Atribuir uma mensagem para o log</para>
        /// <para name="content">Atribuir o conteúdo do log</para>
        /// </summary>  
        public void LogContent(object content, string? message = null)
        {
            if (Logs.Any())
            {
                var item = Logs.LastOrDefault();
                item.LogsContent?.Add(new LogContent(
                    message,
                    content
                ));
                item.Success = true;
            }
        }

        /// <summary>
        /// <para name="exception">Atribuir um exception para o log</para>
        /// <para name="message">Atribuir uma mensagem para o log</para>
        /// </summary>  
        public void LogError(Exception exception, string? message = null)
        {
            if (Logs.Any())
            {
                var item = Logs.LastOrDefault();
                item.LogsContent?.Add(new LogContent(
                    string.IsNullOrEmpty(message) is true ? exception.TargetSite.ToString() : $"{message}\n{exception.TargetSite}",
                    exception.ToString()
                ));
                item.Success = false;
            }
        }

        /// <summary>
        /// Salvar conteúdo do log no banco
        /// <para name="log">Adicionar modelo do log</para>
        /// </summary>  
        public async static Task SaveLogsAsync(LLogger log)
        {
            try
            {
                log.Id = ObjectId.GenerateNewId().ToString();
                var collection = await LLConnection.GetCollectionAsync();
                await collection.InsertOneAsync(log);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Consultar log no banco
        /// </summary>  
        public async static Task<List<LLogger>> SearchLogsAsync(DateTime? startDate, DateTime? endDate, string? projectId = null, string? logId = null, string? contentId = null, int skip = 0, int take = 25, string? projectName = null, string? logName = null, string? uniqueId = null, bool? success = null)
        {
            try
            {
                var collection = await LLConnection.GetCollectionAsync();
                var query = collection.AsQueryable().Where(x => x.Date >= startDate && x.Date <= endDate);

                if (!string.IsNullOrEmpty(projectId))
                    query = query.Where(x => x.Id == projectId);  

                if (!string.IsNullOrEmpty(projectName))
                    query = query.Where(x => x.ProjectName == projectName);

                if (!string.IsNullOrEmpty(logId))
                    query = query.Where(x => x.Logs.Any(i => i.Id == logId));

                if (!string.IsNullOrEmpty(logName))
                    query = query.Where(x => x.Logs.Any(i => i.LogName == logName));

                if (success != null)
                    query = query.Where(x => x.Logs.Any(i => i.Success == success));

                if (!string.IsNullOrEmpty(uniqueId))
                    query = query.Where(x => x.Logs.Any(i => i.UniqueId == uniqueId));

                if (!string.IsNullOrEmpty(contentId))
                    query = query.Where(x => x.Logs.Any(i => i.LogsContent.Any(c => c.Id == contentId)));

                return query.Skip(skip).Take(take).OrderByDescending(x => x.Date).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class Log
    {
        [BsonId]
        public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? LogName { get; set; } = default!;
        public string? Environment { get; set; } = default!;
        public string? UniqueId { get; set; } = default!;
        public string? Description { get; set; } = default!;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
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

        [BsonId]
        public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? Message { get; set; } = default!;
        public object? Content { get; set; } = default!;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreatedAt { get; set; } = default!;
    }
}