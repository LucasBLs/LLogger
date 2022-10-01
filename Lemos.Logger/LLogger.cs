using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lemos.Logger.Model;
using MongoDB.Driver;

namespace Lemos.Logger
{
    public static class LLogger
    {
        public async static Task SaveLogs(LLoggerModel log)
        {
            try
            {
                log.Id = Guid.NewGuid();
                var collection = await LLConnection.GetCollectionAsync();
                await collection.InsertOneAsync(log);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async static Task<List<LLoggerModel>> SearchLogs(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10)
        {
            try
            {
                var collection = await LLConnection.GetCollectionAsync();
                var filter = Builders<LLoggerModel>.Filter.Where(x => x.Date >= startDate && x.Date <= endDate);
                return await collection.Find(filter).Skip((page - 1) * pageSize).Limit(pageSize).SortByDescending(a => a.Date).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async void Teste()
        {
            await LLConnection.ConfigureDatabaseAsync("mongodb://localhost:27017", "teste");

            var llogger = new LLoggerModel("Projeto01");
            llogger.LogFunction("JobTeste01", "Dev", "o123456", "Job teste");
            llogger.LogContent("Criando item no banco", new Exception("Teste"));
            llogger.LogContent("Salvando dados", new SocketException());
            await SaveLogs(llogger);
        }
    }
}
