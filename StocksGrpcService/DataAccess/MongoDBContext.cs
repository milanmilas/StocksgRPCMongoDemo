using MongoDB.Driver;
using StocksGrpcService.Configurations;
namespace StocksGrpcService.DataAccess
{
    public class MongoDBContext<T> : IMongoDBContext<T>
    {
        private IMongoDatabase _db { get; set; }
        private MongoClient _mongoClient { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoDBContext(DatabaseConfiguration configuration)
        {
            _mongoClient = new MongoClient(configuration.ConnectionString);
            _db = _mongoClient.GetDatabase(configuration.DatabaseName);
        }

        public IMongoCollection<T> GetCollection(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}
