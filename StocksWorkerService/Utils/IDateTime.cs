using System;

namespace StocksWorkerService.Utils
{
    public interface IDateTime
    {
        DateTime UtcNow();
    }
}