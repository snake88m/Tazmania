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
    public class IrrigationService : BackgroundServiceBase<IrrigationService>
    {
        readonly IIrrigationRepository IrrigationRepository;

        public IrrigationService(ILogger<IrrigationService> logger,
                                 IMemoryService memoryService,
                                 IIrrigationRepository irrigationRepository) : base(logger, memoryService)
        {
            IrrigationRepository = irrigationRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartDelay(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var settings = await IrrigationRepository.FetchSettings();
                    var irrigations = await IrrigationRepository.Fetchs();

                    if (settings.IsStarted)
                    {
                        // imposto il giorno attuale come data di ultima esecuzione
                        await IrrigationRepository.SetExecution();
                        await IrrigationRepository.SaveChangesAsync();

                        // faccio effettivamente partire l'irrigazione solo se non ha piovuto
                        if (!MemoryService.GetInput(settings.RainSensorId).IsActive)
                        {
                            foreach (var irrigation in irrigations)
                            {
                                IO sprinkler = MemoryService.GetOutput(irrigation.OutputSprinklerId);

                                MemoryService.SetOutput(sprinkler.Id, true);
                                Logger.LogIrrigation(true, irrigation);

                                await Task.Delay(TimeSpan.FromMinutes(irrigation.Minutes), stoppingToken);

                                MemoryService.SetOutput(sprinkler.Id, false);
                                Logger.LogIrrigation(false, irrigation);
                            }
                        }
                        else
                        {
                            Logger.LogInformation("[IRRIGATION] not started - rain sensor actived");
                        }
                    }
                    else
                    {
                        // spengo eventuali irrigatori accesi e che non sono in fase di test
                        foreach (var sprinkler in MemoryService.IOs.Where(r => irrigations.Where(r => !r.IsWateringInProgress).Select(s => s.OutputSprinklerId).Contains(r.Id) && r.IsActive))
                        {
                            MemoryService.SetOutput(sprinkler.Id, false);
                            Logger.LogIrrigation(false, irrigations.Single(r => r.OutputSprinklerId == sprinkler.Id));
                        }

                        // accendo gli irrigatori per cui è stato richiesto un test
                        foreach (var sprinkler in MemoryService.IOs.Where(r => irrigations.Where(r => r.IsWateringInProgress).Select(s => s.OutputSprinklerId).Contains(r.Id) && !r.IsActive))
                        {
                            MemoryService.SetOutput(sprinkler.Id, true);
                            Logger.LogIrrigation(true, irrigations.Single(r => r.OutputSprinklerId == sprinkler.Id));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Irrigation error");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
