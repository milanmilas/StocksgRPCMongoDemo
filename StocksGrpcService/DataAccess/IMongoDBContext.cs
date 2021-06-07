using MongoDB.Driver;

namespace StocksGrpcService.DataAccess
{
    public interface IMongoDBContext<T>
    {
        IClientSessionHandle Session { get; set; }

        IMongoCollection<T> GetCollection(string name);
    }
}