using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Mobile.Models;
using Tazmania.Mobile.Services;

namespace Tazmania.Mobile.ViewModels
{
    public class SecurityViewModel : BaseViewModel
    {
        readonly SecurityRestService securityRestService;

        string antiFireDetail;
        string sosDetail;
        string antitheftDetail;
        string bottomDescription;

        public ObservableCollection<IO> Antithefts { get; set; } = new();

        public ObservableCollection<IO> Options { get; set; } = new();

        public string AntiFireDetail
        {
            get { return antiFireDetail; }
            set { antiFireDetail = value; OnPropertyChanged(nameof(AntiFireDetail)); }
        }

        public string SOSDetail
        {
            get { return sosDetail; }
            set { sosDetail = value; OnPropertyChanged(nameof(SOSDetail)); }
        }

        public string AntitheftDetail
        {
            get { return antitheftDetail; }
            set { antitheftDetail = value; OnPropertyChanged(nameof(AntitheftDetail)); }
        }

        public string BottomDescription
        {
            get { return bottomDescription; }
            set
            {
                bottomDescription = value;
                OnPropertyChanged(nameof(BottomDescription));
            }
        }

        public Command DeactiveAllarms { get; set; }

        public SecurityViewModel(SecurityRestService securityRestService) 
        {
            this.securityRestService = securityRestService;

            Antithefts.Add(new IO() { Id = 11, Major = "antithefttotal", Description = "Totale" });
            Antithefts.Add(new IO() { Id = 12, Major = "antitheftpartial", Description = "Parziale" });

            Options.Add(new IO() { Id = 1, Major = "siren", Description = "Sirena" });
            Options.Add(new IO() { Id = 2, Major = "dialer", Description = "Combinatore" });
            Options.Add(new IO() { Id = 3, Major = "smokesensor", Description = "Test Antincendio" });
            Options.Add(new IO() { Id = 4, Major = "emergencybutton", Description = "Test SOS" });

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await this.securityRestService.WaitWhenIsReady();

                    try
                    {
                        var response = await this.securityRestService.Fetchs();
                        Antithefts.Single(r => r.Id == 11).IsActive = (response.AntitheftMode == 2);
                        Antithefts.Single(r => r.Id == 12).IsActive = (response.AntitheftMode == 3);
                        Options.Single(r => r.Id == 1).IsActive = response.NotifySiren;
                        Options.Single(r => r.Id == 2).IsActive = response.NotifyCall;
                        AntiFireDetail = response.AntiFireActive ? response.AntiFireDetail : string.Empty;
                        SOSDetail = response.SOSActive ? response.SOSDetail : string.Empty;
                        AntitheftDetail = response.AntitheftActive ? response.AntitheftDetail : string.Empty;

                        if (response.OpenGates.Any())
                        {
                            BottomDescription = $"I seguenti varchi sono aperti:\n{string.Join("\n", response.OpenGates.Select(r => $"{r.GroupName} - {r.Description}"))}";
                        }
                        else
                        {
                            BottomDescription = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    await Task.Delay(1000);
                }
            });

            DeactiveAllarms = new Command(async _ =>
            {
                await securityRestService.DeactiveAlarms();
            });
        }

        public async void ToggleIsActive(IO io)
        {
            io.IsBusy = true;

            switch (io.Id)
            {
                case 1: await securityRestService.SetNotifySiren(!io.IsActive); break;
                case 2: await securityRestService.SetNotifyCall(!io.IsActive); break;
                case 3: await securityRestService.ActiveAntiFire(); break;
                case 4: await securityRestService.ActiveSOS(); break;
                case 11: await securityRestService.SetAntitheftMode(!io.IsActive ? 2 : 1); break;
                case 12: await securityRestService.SetAntitheftMode(!io.IsActive ? 3 : 1); break;
            }

            await Task.Delay(3000);
            io.IsBusy = false;
        }
    }
}
