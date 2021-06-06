using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService
{
    public static class CancellationTokenExtensions
    {
        public static Task AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource();
            cancellationToken.Register(() => { tcs.TrySetCanceled(); });
            return tcs.Task;
        }
    }
}
