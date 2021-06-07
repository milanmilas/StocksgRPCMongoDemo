using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using StocksWorkerService.Configurations;
using StocksWorkerService.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Schedulers
{
    public class Scheduler<T>: IScheduler where T : SchedulerConfiguration
    {
        private readonly ILogger<Scheduler<T>> _logger;
        private readonly IStocksFeeder<T> _feeder;
        private readonly T _config;
        private Quartz.IScheduler _scheduler;

        public Scheduler(ILogger<Scheduler<T>> logger,IStocksFeeder<T> feeder, T config)
        {
            _logger = logger;
            _feeder = feeder;
            _config = config;
        }
        public void Start(CancellationToken cancellationToken)
        {
            var schedFact = new StdSchedulerFactory();
            _scheduler = schedFact.GetScheduler().Result;
            _scheduler.Start();

            Func<CancellationToken, Task> func = _feeder.Proccess;

            IJobDetail job = JobBuilder.Create<FeederJob<T>>()
                .SetJobData(new JobDataMap
                {
                    {"func", func },
                    {"token", cancellationToken }
                })
                .WithIdentity(typeof(T).Name)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule("0 0/1 * * * ?")
                .ForJob(job)
                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }
        public void Stop()
        {
            _scheduler?.Start();
        }
    }
}
