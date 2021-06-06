using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StocksWorkerService.Utils
{
    public class QuartzScheduler<T>
    {
        private readonly IScheduler scheduler;
        private readonly string name;
        private readonly string cronSchedule;
        private readonly Func<CancellationToken, Task> func;
        private readonly CancellationToken cancellationToken;

        class ActionJob : IJob
        {
            public Func<CancellationToken, Task> Action { get; set; }
            public CancellationToken cancellationToken { get; set; }

            public async Task Execute(IJobExecutionContext context)
            {
                await Action?.Invoke(cancellationToken);
            }
        }

        public QuartzScheduler(string name,
            ISchedulerFactory schedulerFactory,
            string cronSchedule,
            Func<CancellationToken, Task> func)
        {
            scheduler = schedulerFactory.GetScheduler().Result;
            this.name = name;
            this.cronSchedule = cronSchedule;
            this.func = func;
        }

        public Task<DateTimeOffset> Start(CancellationToken cancellationToken)
        {
            IJobDetail job = JobBuilder.Create<ActionJob>()
                .WithIdentity(name)
                .Build();

            (job as ActionJob).Action = func;

            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(cronSchedule)
                .ForJob(job)
                .Build();

            return scheduler.ScheduleJob(job, trigger);
        }

        public Task Stop()
        {
            return scheduler.Shutdown();
        }
    }
}
