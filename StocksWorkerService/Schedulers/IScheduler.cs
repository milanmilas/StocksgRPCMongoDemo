using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Schedulers
{
    public interface IScheduler
    {
        void Start(CancellationToken cancellationToken);
        void Stop();
    }
}
