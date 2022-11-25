using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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

                var objectDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
                var objectSerializer = new ObjectSerializer(objectDiscriminatorConvention, GuidRepresentation.CSharpLegacy);
                BsonSerializer.RegisterSerializer(objectSerializer);

                ConnectionString = connectionString;
                CollectionName = collectionName;
                var database = new MongoClient(ConnectionString).GetDatabase(DataBaseName);
                var options = new ListCollectionNamesOptions
                {
                    Filter = new BsonDocument("name", CollectionName)
                };

                var existCollection = await database.ListCollectionNamesAsync(options);
                if (!existCollection.Any())
                {
                    await database.CreateCollectionAsync(CollectionName);

                    var collection = database.GetCollection<LLogger>(CollectionName);
                    var indexLLogger = Builders<LLogger>.IndexKeys
                        .Ascending(i => i.ProjectName)
                        .Ascending(i => i.Date)
                        .Ascending(i => i.Logs[0].LogName)
                        .Ascending(i => i.Logs[0].Environment)
                        .Ascending(i => i.Logs[0].Success)
                        .Ascending(i => i.Logs[0].CreatedAt);

                    await collection.Indexes.CreateOneAsync(new CreateIndexModel<LLogger>(indexLLogger));
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async static Task<IMongoCollection<LLogger>> GetCollectionAsync()
        {
            try
            {
                var database = new MongoClient(ConnectionString).GetDatabase(DataBaseName);
                return await Task.FromResult(database.GetCollection<LLogger>(CollectionName));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}