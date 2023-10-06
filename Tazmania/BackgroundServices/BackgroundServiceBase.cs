using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;

namespace Tazmania.BackgroundServices
{
    public class BackgroundServiceBase<T> : BackgroundService
    {
        protected readonly ILogger<T> Logger;
        protected readonly IMemoryService MemoryService;
         
        protected BackgroundServiceBase(ILogger<T> logger,
                                        IMemoryService memoryService)
        {
            Logger = logger;
            MemoryService = memoryService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Deve essere obbligatoriamente implementato da chi eredita
            throw new NotImplementedException();
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{typeof(T)} Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }

        /// <summary>
        /// Genera un tempo di attesa prima di avviare l'esecuzione
        /// Da usare per attendere che il servizio di automazione sia avviato
        /// </summary>
        protected async Task StartDelay(CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
