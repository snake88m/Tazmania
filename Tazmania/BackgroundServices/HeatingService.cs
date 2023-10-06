using Microsoft.Extensions.Hosting;
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
    /// <summary>
    /// Gestisce l'impianto di riscaldamento
    /// </summary>
    public class HeatingService : BackgroundServiceBase<HeatingService>
    {
        readonly IHeatingRepository HeatingRepository;

        public HeatingService(ILogger<HeatingService> logger,
                              IHeatingRepository heatingRepository,
                              IMemoryService memoryService) :  base(logger, memoryService)
        {
            HeatingRepository = heatingRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartDelay(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var settings = await HeatingRepository.FetchSettings();
                    var heatings = await HeatingRepository.Fetchs();

                    if (settings.IsStarted)
                    {
                        foreach (var heating in heatings)
                        {
                            IO sensor = MemoryService.GetInput(heating.InputSensorId);
                            IO valve = MemoryService.GetOutput(heating.OutputValveId);

                            if (valve.IsActive)
                            {
                                // spengo solo se raggiungo temperatura + offset
                                if (sensor.ValueCorrected >= heating.Temperature + settings.Offset)
                                {
                                    MemoryService.SetOutput(valve.Id, false);
                                    Logger.LogHeating(false, heating, settings, sensor);
                                }
                            }
                            else
                            {
                                if (sensor.ValueCorrected < heating.Temperature)
                                {
                                    MemoryService.SetOutput(valve.Id, true);
                                    Logger.LogHeating(true, heating, settings, sensor);
                                }
                            }
                        }
                    }
                    else
                    {
                        // spengo tutte le valvole se attualmente accese
                        foreach (var valve in MemoryService.IOs.Where(r => heatings.Select(s => s.OutputValveId).Contains(r.Id) && r.IsActive))
                        {
                            MemoryService.SetOutput(valve.Id, false);
                            Logger.LogHeating(false, heatings.Single(r => r.OutputValveId == valve.Id), settings);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Heating error");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
