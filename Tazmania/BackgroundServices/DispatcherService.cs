using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;
using Tazmania.Services;
using Tazmania.Extensions;

namespace Tazmania.BackgroundServices
{
    /// <summary>
    /// Elabora le richieste pervenute dall'esterno
    /// </summary>
    public class DispatcherService : BackgroundServiceBase<DispatcherService>
    {
        readonly IRequestRepository RequestRepository;
        readonly IHeatingRepository HeatingRepository;
        readonly IIrrigationRepository IrrigationRepository;
        readonly ISecurityRepository SecurityRepository;

        public DispatcherService(ILogger<DispatcherService> logger,
                                 IMemoryService memoryService,
                                 IRequestRepository requestRepository,
                                 IHeatingRepository heatingRepository,
                                 IIrrigationRepository irrigationRepository,
                                 ISecurityRepository securityRepository) : base(logger, memoryService)
        {
            RequestRepository = requestRepository;
            HeatingRepository = heatingRepository;
            IrrigationRepository = irrigationRepository;
            SecurityRepository = securityRepository;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartDelay(stoppingToken);

            try
            {
                // al primo avvio pulisco il database da eventuali richieste già esistenti
                await RequestRepository.FetchsAndClean();
                await RequestRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Start clean dispatcher error");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // leggo le richieste attualmente in sospeso
                    var requests = await RequestRepository.FetchsAndClean();
                    await RequestRepository.SaveChangesAsync();

                    foreach (var request in requests)
                    {
                        if (request.Type == RequestType.AutomationSetOutput)
                        {
                            // salvo le richieste in memoria, NON le salvo su db perchè lo farà l'AutomationService
                            MemoryService.SetOutput(request.EntityId, request.IsActive);
                        }
                        else if (request.Type == RequestType.HeatingSetTemperature)
                        {
                            await HeatingRepository.SetTemperature(request.EntityId, request.Value);
                            await HeatingRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.HeatingSetMode)
                        {
                            await HeatingRepository.SetMode((HeatingMode)request.Value);
                            await HeatingRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.SecurityActiveAntiFire)
                        {
                            await SecurityRepository.ActiveAntiFire("Attivazione esterna");
                            await SecurityRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.SecurityActiveSOS)
                        {
                            await SecurityRepository.ActiveSOS("Attivazione esterna");
                            await SecurityRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.SecurityDeactiveAlarms)
                        {
                            await SecurityRepository.DeactiveAlarms();
                            await SecurityRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.SecurityAntitheftSetMode)
                        {
                            await SecurityRepository.SetAntitheftMode((SecurityAntitheftMode)request.Value);
                            await SecurityRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.SecurityNotifyCallSet)
                        {
                            await SecurityRepository.SetNotifyCall(request.IsActive);
                            await SecurityRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.SecurityNotifySirenSet)
                        {
                            await SecurityRepository.SetNotifySiren(request.IsActive);
                            await SecurityRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.IrrigationSetWeekDays)
                        {
                            await IrrigationRepository.SetWeekDays((DayOfWeek)request.EntityId, request.IsActive);
                            await IrrigationRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.IrrigationSetMode)
                        {
                            await IrrigationRepository.SetMode((IrrigationMode)request.Value);
                            await IrrigationRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.IrrigationSetWatering)
                        {
                            await IrrigationRepository.SetWatering(request.EntityId, request.IsActive);
                            await IrrigationRepository.SaveChangesAsync();
                        }
                        else if (request.Type == RequestType.IrrigationSetTimer)
                        {
                            await IrrigationRepository.SetTimer(request.EntityId, (int)request.Value);
                            await IrrigationRepository.SaveChangesAsync();
                        }

                        Logger.LogRequest(request);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Dispatcher error");
                }

                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
