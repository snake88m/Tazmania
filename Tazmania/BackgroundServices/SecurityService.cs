using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Extensions;
using Tazmania.Interfaces.Automation;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;
using Tazmania.Services;

namespace Tazmania.BackgroundServices
{
    public class SecurityService : BackgroundServiceBase<SecurityService>
    {
        readonly ISecurityRepository SecurityRepository;
        readonly IMailService MailService;
        readonly INotifyService NotifyService;

        public SecurityService(ILogger<SecurityService> logger,
                               ISecurityRepository securityRepository,
                               IMemoryService memoryService,
                               IMailService mailService,
                               INotifyService notifyService) : base(logger, memoryService)
        {
            SecurityRepository = securityRepository;
            MailService = mailService;
            NotifyService = notifyService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool sosHandled = false;
            bool antiFireHandled = false;
            bool antitheftHandled = false;

            await StartDelay(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var setting = await SecurityRepository.FetchSettings();

                    var buttons = MemoryService.IOs.Where(r => r.Major == IOMajor.EmergencyButton &&
                                                               !r.Input.Handled &&
                                                               r.Input.TotalAction >= 3);

                    if (buttons.Any())
                    {
                        foreach (var button in buttons)
                        {
                            button.Input.Handled = true;
                            await SecurityRepository.ActiveSOS(button.Description);
                            await SecurityRepository.SaveChangesAsync();
                        }
                    }

                    if (setting.AntiFire)
                    {
                        var sensors = MemoryService.IOs.Where(r => r.Major == IOMajor.SmokeSensor &&
                                                                   !r.Input.Handled);

                        foreach (var sensor in sensors)
                        {
                            sensor.Input.Handled = true;
                            await SecurityRepository.ActiveAntiFire(sensor.Description);
                            await SecurityRepository.SaveChangesAsync();
                        }
                    }

                    if (setting.AntitheftMode != SecurityAntitheftMode.Total)
                    {
                        var majors = new List<IOMajor>() { IOMajor.WindowSensor };

                        if (setting.AntitheftMode == SecurityAntitheftMode.Disabled)
                        {
                            majors.Add(IOMajor.DoorSensor);
                        }

                        foreach (var io in MemoryService.IOs.Where(r => majors.Contains(r.Major) && !r.Input.Handled))
                        {
                            io.Input.Handled = true;
                        }
                    }

                    if (setting.AntitheftMode != SecurityAntitheftMode.Disabled)
                    {
                        var sensors = MemoryService.IOs.Where(r => r.Major == IOMajor.DoorSensor &&
                                                                              !r.Input.Handled &&
                                                                              r.Input.TotalAction >= 3);

                        if (setting.AntitheftMode == SecurityAntitheftMode.Total)
                        {
                            sensors = sensors.Concat(MemoryService.IOs.Where(r => r.Major == IOMajor.WindowSensor &&
                                                                                  !r.Input.Handled &&
                                                                                  r.Input.TotalAction >= 3));
                        }

                        foreach (var sensor in sensors)
                        {
                            sensor.Input.Handled = true;
                            await SecurityRepository.ActiveAntitheft(sensor.Description);
                            await SecurityRepository.SaveChangesAsync();
                        }
                    }

                    if (setting.SOSActive && !sosHandled) 
                    {
                        sosHandled = true;

                        SendMail("Questa è una richiesta di soccorso", setting.SOSDetail);

                        if (setting.NotifyCall)
                        {
                            NotifyService.SendSOSDialer();
                        }

                        NotifyService.PlayInternalSiren();

                        if (setting.NotifySiren)
                        {
                            NotifyService.PlaySirens();
                        }

                        Logger.LogInformation($"Avviata richiesta di soccorso: {setting.SOSDetail}");
                    }
                    else if (!setting.SOSActive)
                    {
                        sosHandled = false;
                    }

                    if (setting.AntiFireActive && !antiFireHandled)
                    {
                        antiFireHandled = true;

                        SendMail("Allarme antincendio attivo", setting.AntiFireDetail);

                        if (setting.NotifyCall)
                        {
                            NotifyService.SendAutomaticDialer();
                        }

                        NotifyService.PlayInternalSiren();

                        if (setting.NotifySiren)
                        {
                            NotifyService.PlaySirens();
                        }

                        Logger.LogInformation($"Allarme antincendio attivo: {setting.AntiFireDetail}");
                    }
                    else if (!setting.AntiFireActive)
                    {
                        antiFireHandled = false;
                    }

                    if (setting.AntitheftActive && !antitheftHandled)
                    {
                        antitheftHandled = true;

                        SendMail("Allarme intrusione", setting.AntitheftDetail);

                        if (setting.NotifyCall)
                        {
                            NotifyService.SendAutomaticDialer();
                        }

                        NotifyService.PlayInternalSiren();

                        if (setting.NotifySiren)
                        {
                            NotifyService.PlaySirens();
                        }

                        Logger.LogInformation($"Allarme antincendio attivo: {setting.AntiFireDetail}");
                    }
                    else if (!setting.AntitheftActive)
                    {
                        antitheftHandled = false;
                    }

                    // disattivo sirene e dialer se gli allarmi vengono disattivati
                    if (!setting.SOSActive && !setting.AntiFireActive && !setting.AntitheftActive)
                    {
                        NotifyService.StopSirens();
                        NotifyService.StopInternalSirens();
                    }
                    
                    if (!setting.SOSActive)
                    {
                        NotifyService.StopSOSDialer();
                    }
                    
                    if (!setting.AntiFireActive && !setting.AntitheftActive)
                    {
                        NotifyService.StopAutomaticDialer();
                    }

                    // dopo 30 minuti l'allarme si spegne in automatico
                    if (setting.SOSActive && DateTime.Now > setting.SOSActivationDateTime.AddMinutes(30))
                    {
                        await SecurityRepository.DeactiveSOS();
                        await SecurityRepository.SaveChangesAsync();
                    }

                    if (setting.AntiFireActive && DateTime.Now > setting.AntiFireActivationDateTime.AddMinutes(30))
                    {
                        await SecurityRepository.DeactiveAntiFire();
                        await SecurityRepository.SaveChangesAsync();
                    }

                    if (setting.AntitheftActive && DateTime.Now > setting.AntitheftActivationDateTime.AddMinutes(30))
                    {
                        await SecurityRepository.DeactiveAntitheft();
                        await SecurityRepository.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Security error");
                }

                await Task.Delay(500, stoppingToken);
            }
        }

        void SendMail(string evn, string detail)
        {
            var mailObject = new MailObject()
            {
                Subject = "Notifica di sicurezza",
                Template = "security-notify",
                Messages = new Dictionary<string, string>()
                {
                    { "##EVENT##", evn },
                    { "##DETAIL##", detail.ToUpper() },
                    { "##DATETIME##", DateTime.Now.AsString() }
                }
            };

            MailService.SendAsync(mailObject);

            Logger.LogInformation($"Inviata mail notifica {evn} {detail}");
        }
    }
}
