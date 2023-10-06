using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Extensions;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;

namespace Tazmania.BackgroundServices
{
    public class SchedulerService : BackgroundServiceBase<SchedulerService>
    {
        readonly ISchedulerRepository SchedulerRepository;

        public SchedulerService(ILogger<SchedulerService> logger,
                                ISchedulerRepository schedulerRepository,
                                IMemoryService memoryService) : base(logger, memoryService)
        {
            SchedulerRepository = schedulerRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartDelay(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var scheduler in await SchedulerRepository.Fetchs())
                    {
                        foreach (var item in scheduler.Items)
                        {
                            MemoryService.SetOutput(item.OutputId, scheduler.IsStarted);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Scheduler error");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
