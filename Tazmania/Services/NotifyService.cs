using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;
using Tazmania.Extensions;

namespace Tazmania.Services
{
    public class NotifyService : INotifyService
    {
        const int PLAY_SIRENS_REPEAT = 3;
        const int PLAY_INTERNAL_SIRENS_REPEAT = 10;

        readonly ILogger<NotifyService> Logger;
        readonly INotifyRepository NotifyRepository;
        readonly IMemoryService MemoryService;

        CancellationTokenSource? sirenCTS = null;
        CancellationTokenSource? internalSirenCTS = null;
        CancellationTokenSource? dialerSOSCTS = null;
        CancellationTokenSource? dialerAutomaticCTS = null;

        public NotifyService(ILogger<NotifyService> logger, 
                             INotifyRepository notifyRepository,
                             IMemoryService memoryService)
        {
            Logger = logger;
            NotifyRepository = notifyRepository;
            MemoryService = memoryService;
        }

        public void PlaySirens()
        {
            if (sirenCTS != null)
            {
                Logger.LogInformation("PlaySirens already running");
                return;
            }

            sirenCTS = new CancellationTokenSource();

            var ios = MemoryService.IOs.Where(r => r.Major == IOMajor.Siren);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < PLAY_SIRENS_REPEAT; i++)
                {
                    ios.ForEach(r => MemoryService.SetOutput(r.Id, true));
                    await Task.Delay(60000, sirenCTS.Token);

                    ios.ForEach(r => MemoryService.SetOutput(r.Id, false));
                    await Task.Delay(60000, sirenCTS.Token);
                }
            }, sirenCTS.Token)
            .ContinueWith(t =>
            {
                sirenCTS = null;
                ios.ForEach(r => MemoryService.SetOutput(r.Id, false));

                if (t.IsFaulted)
                {
                    Logger.LogError("PlaySirens Task failed");
                }
            });
        }

        public void StopSirens()
        {
            sirenCTS?.Cancel();
        }

        public void PlayInternalSiren()
        {
            if (internalSirenCTS != null)
            {
                Logger.LogInformation("PlayInternalSirens already running");
                return;
            }

            internalSirenCTS = new CancellationTokenSource();

            var ios = MemoryService.IOs.Where(r => r.Major == IOMajor.InternalSiren);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < PLAY_INTERNAL_SIRENS_REPEAT; i++)
                {
                    ios.ForEach(r => MemoryService.SetOutput(r.Id, true));
                    await Task.Delay(30000, internalSirenCTS.Token);

                    ios.ForEach(r => MemoryService.SetOutput(r.Id, false));
                    await Task.Delay(30000, internalSirenCTS.Token);
                }
            }, internalSirenCTS.Token)
            .ContinueWith(t =>
            {
                internalSirenCTS = null;
                ios.ForEach(r => MemoryService.SetOutput(r.Id, false));

                if (t.IsFaulted)
                {
                    Logger.LogError("PlayInternalSirens Task failed");
                }
            });
        }

        public void StopInternalSirens()
        {
            internalSirenCTS?.Cancel();
        }

        public async void SendSOSDialer()
        {
            if (dialerSOSCTS != null)
            {
                Logger.LogInformation("Dialer SOS already running");
                return;
            }

            dialerSOSCTS = new CancellationTokenSource();
            var settings = await NotifyRepository.FetchSettings();

            _ = Task.Run(async () =>
            {
                // tempo di attesa per disinserimento in caso di azione involontaria
                await Task.Delay(15000, dialerSOSCTS.Token);
                MemoryService.SetOutput(settings.DialerSOSId, true);
                await Task.Delay(10000, dialerSOSCTS.Token);
                
            }, dialerSOSCTS.Token)
            .ContinueWith(t =>
            {
                dialerSOSCTS = null;
                MemoryService.SetOutput(settings.DialerSOSId, false);

                if (t.IsFaulted)
                {
                    Logger.LogError("SendSOSDialer Task failed");
                }
            });
        }

        public void StopSOSDialer()
        {
            dialerSOSCTS?.Cancel();
        }

        public async void SendAutomaticDialer()
        {
            if (dialerAutomaticCTS != null)
            {
                Logger.LogInformation("Dialer Automatic already running");
                return;
            }

            dialerAutomaticCTS = new CancellationTokenSource();

            var settings = await NotifyRepository.FetchSettings();

            _ = Task.Run(async () =>
            {
                // tempo di attesa per disinserimento in caso di azione involontaria
                await Task.Delay(15000, dialerAutomaticCTS.Token);
                MemoryService.SetOutput(settings.DialerAutomaticId, true);
                await Task.Delay(10000, dialerAutomaticCTS.Token);

            }, dialerAutomaticCTS.Token)
            .ContinueWith(t =>
            {
                dialerAutomaticCTS = null;
                MemoryService.SetOutput(settings.DialerAutomaticId, false);

                if (t.IsFaulted)
                {
                    Logger.LogError("SendSOSDialer Task failed");
                }
            });
        }

        public void StopAutomaticDialer()
        {
            dialerAutomaticCTS?.Cancel();
        }
    }
}
