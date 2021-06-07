using System.Collections.Generic;

namespace StocksGrpcService
{
    public interface IConverter<T, V>
    {
        public List<V> Convert(IEnumerable<T> items);
    }
}
