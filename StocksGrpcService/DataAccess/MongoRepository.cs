using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StocksGrpcService.DataAccess.Attributes;
using StocksGrpcService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksGrpcService.DataAccess
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IDocument
    {
        private IMongoCollection<T> _collection;
        private readonly ILogger<MongoRepository<T>> _logger;
        private readonly IMongoDBContext<T> _dBContext;

        public MongoRepository(ILogger<MongoRepository<T>> logger, IMongoDBContext<T> dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
            _collection = _dBContext.GetCollection(GetCollectionName(typeof(T)));
        }

        public void InsertMany(ICollection<T> documents)
        {
            try
            {
                _logger.LogDebug("Start Inserting Many.");
                _collection.InsertMany(documents);
                _logger.LogDebug("End Inserting Many.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has occured while inserting documents.");
                throw;
            }
        }


        public virtual async Task InsertManyAsync(ICollection<T> documents)
        {
            try
            {
                _logger.LogDebug("Start Inserting Many Async.");
                await _collection.InsertManyAsync(documents);
                _logger.LogDebug("End Inserting Many.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has occured while inserting documents.");
                throw;
            }
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }
    }
}
