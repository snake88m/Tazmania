using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Automation;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;

namespace Tazmania.BackgroundServices
{
    /// <summary>
    /// Effettua la sincronizzazione dei dati tra server e DFCP
    /// </summary>
    public class AutomationService : BackgroundServiceBase<AutomationService>
    {
        readonly IDuemmegiService DuemmegiService;
        readonly IIORepository IORepository;

        public AutomationService(ILogger<AutomationService> logger,
                                 IDuemmegiService duemmegiService,
                                 IIORepository ioRepository,
                                 IMemoryService memoryService) : base(logger, memoryService)
        {
            DuemmegiService = duemmegiService;
            IORepository = ioRepository;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MemoryService.InitIOs(await IORepository.Fetchs());

            try
            {
                foreach (var io in MemoryService.IOs.Where(r => r.DefaultIsActive.HasValue))
                {
                    MemoryService.SetOutput(io.Id, io.DefaultIsActive.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Default initialized error");
            }

            _ = ReadDFCP(stoppingToken);
            _ = WriteDFCP(stoppingToken);
        }

        async Task ReadDFCP(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DuemmegiService.ReadIO(MemoryService.IOs);

                    foreach (var io in MemoryService.IOs.Where(r => r.Type != IOType.InputValue))
                    {
                        await IORepository.SetActive(io.Id, io.IsActive);
                    }

                    foreach (var io in MemoryService.IOs.Where(r => r.Type == IOType.InputValue))
                    {
                        await IORepository.SetValue(io.Id, io.Value);
                    }

                    await IORepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Read DFCP error");
                }

                await Task.Delay(500, stoppingToken);
            }
        }

        async Task WriteDFCP(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DuemmegiService.WriteOutput(MemoryService.IOs.Where(r => r.Type == IOType.Output));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Write DFCP error");
                }

                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
