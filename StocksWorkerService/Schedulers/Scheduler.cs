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
            try
            {
                var schedFact = new StdSchedulerFactory();
                _scheduler = schedFact.GetScheduler().Result;
                _scheduler.Start();

                Func<CancellationToken, Task> func = _feeder.Proccess;

                IJobDetail job = JobBuilder.Create<FeederJob>()
                    .SetJobData(new JobDataMap
                    {
                    {"func", func },
                    {"token", cancellationToken }
                    })
                    .WithIdentity(typeof(T).Name)
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithCronSchedule(_config.Schedule)
                    .ForJob(job)
                    .Build();

                _logger.LogInformation($"Scheduling job '{typeof(T).Name}' with schedule '{_config.Schedule}'.");
                _scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception e )
            {
                _logger.LogError(e, $"An error has occured scheduling job '{typeof(T).Name}' with message '{e.Message}'.");
            }
            
        }
        public void Stop()
        {
            try
            {
                _scheduler?.Shutdown();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error has occured stopping sheduler '{typeof(T).Name}' with message '{e.Message}'.");
            }            
        }
    }
}
