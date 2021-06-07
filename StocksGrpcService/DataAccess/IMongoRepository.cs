using StocksGrpcService.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksGrpcService.DataAccess
{
    public interface IMongoRepository<T> where T : IDocument
    {
        void InsertMany(ICollection<T> documents);
        Task InsertManyAsync(ICollection<T> documents);
    }
}