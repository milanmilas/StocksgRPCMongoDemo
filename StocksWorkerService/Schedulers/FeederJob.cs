using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Schedulers
{
    public class FeederJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            var func = context.MergedJobDataMap["func"] as Func<CancellationToken,Task>;
            CancellationToken token = (CancellationToken)context.MergedJobDataMap["token"];
            await  func?.Invoke(token);
        }
    }
}
