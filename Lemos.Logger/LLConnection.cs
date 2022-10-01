using System;
using System.Threading.Tasks;
using Lemos.Logger.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lemos.Logger
{
    public class LLConnection
    {
        private static string ConnectionString = default!;
        private static string CollectionName = default!;
        private static readonly string DataBaseName = "LLogger";

        /// <summary>
        /// Atribuir informações para o banco de dados
        /// <param name="connectionString">ConnectioString to database MongoDB</param>    
        /// <param name="collectionName">Name to create collection</param>
        /// </summary>     
        public async static Task ConfigureDatabaseAsync(string connectionString, string collectionName)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(collectionName))
                    throw new ArgumentNullException("Invalid parameters for creating the database.");

                ConnectionString = connectionString;
                CollectionName = collectionName;
                var database = new MongoClient(ConnectionString).GetDatabase(DataBaseName);
                var options = new ListCollectionNamesOptions
                {
                    Filter = new BsonDocument("name", CollectionName)
                };

                var existCollection = await database.ListCollectionNamesAsync(options);
                if (!existCollection.Any())
                    await database.CreateCollectionAsync(CollectionName);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async static Task<IMongoCollection<LLoggerModel>> GetCollectionAsync()
        {
            try
            {
                var database = new MongoClient(ConnectionString).GetDatabase(DataBaseName);
                return await Task.FromResult(database.GetCollection<LLoggerModel>(CollectionName));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}