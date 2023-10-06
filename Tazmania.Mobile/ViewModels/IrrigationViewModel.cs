using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Mobile.Models;
using Tazmania.Mobile.Services;

namespace Tazmania.Mobile.ViewModels
{
    public class IrrigationViewModel : BaseViewModel
    {
        readonly IrrigationRestService irrigationRestService;

        string bottomDescription;
        DateTime MinutesUpdateWait = DateTime.MinValue;

        public ObservableCollection<IrrigationModel> Irrigations { get; set; } = new();

        public ObservableCollection<IO> Options { get; set; } = new();

        public string BottomDescription 
        {
            get { return bottomDescription; }
            set
            {
                bottomDescription = value;
                OnPropertyChanged(nameof(BottomDescription));
            }
        }

        public IrrigationViewModel(IrrigationRestService irrigationRestService)
        {
            this.irrigationRestService = irrigationRestService;

            _ = Task.Run(async () =>
            {
                bool fullData = true;

                await this.irrigationRestService.WaitWhenIsReady();

                while (true)
                {
                    try
                    {
                        var response = await this.irrigationRestService.Fetchs(fullData);

                        if (fullData)
                        {
                            foreach (var irrigation in response.Irrigations)
                            {
                                Irrigations.Add(new IrrigationModel()
                                {
                                    Id = irrigation.Id,
                                    Description = irrigation.Description,
                                    Major = "sprinkler",
                                });
                            }

                            Options.Add(new IO() { Id = 1, Description = "Modo automatico", Major = "sprinkler" });
                            Options.Add(new IO() { Id = 2, Description = "Sensore pioggia", Major = "rainsensor", IsReadOnly = true });
                            Options.Add(new IO() { Id = 3, Description = "Lunedì", Major = "monday" });
                            Options.Add(new IO() { Id = 4, Description = "Martedì", Major = "tuesday" });
                            Options.Add(new IO() { Id = 5, Description = "Mercoledì", Major = "wednesday" });
                            Options.Add(new IO() { Id = 6, Description = "Giovedì", Major = "thursday" });
                            Options.Add(new IO() { Id = 7, Description = "Venerdì", Major = "friday" });
                            Options.Add(new IO() { Id = 8, Description = "Sabato", Major = "saturday" });
                            Options.Add(new IO() { Id = 9, Description = "Domenica", Major = "sunday" });

                            fullData = false;
                        }

                        foreach (var irrigation in Irrigations)
                        {
                            if (DateTime.Now > MinutesUpdateWait)
                            {
                                irrigation.Minutes = $"{response.Irrigations.Single(r => r.Id == irrigation.Id).Minutes} minuti";
                            }

                            irrigation.IsActive = response.Irrigations.Single(r => r.Id == irrigation.Id).IsActive;
                        }

                        foreach (var option in Options)
                        {
                            Options.Single(r => r.Id == 1).IsActive = (response.Mode == 1);
                            Options.Single(r => r.Id == 2).IsActive = response.RainSensor;
                            Options.Single(r => r.Id == 3).IsActive = response.Monday;
                            Options.Single(r => r.Id == 4).IsActive = response.Tuesday;
                            Options.Single(r => r.Id == 5).IsActive = response.Wednesday;
                            Options.Single(r => r.Id == 6).IsActive = response.Thursday;
                            Options.Single(r => r.Id == 7).IsActive = response.Friday;
                            Options.Single(r => r.Id == 8).IsActive = response.Saturday;
                            Options.Single(r => r.Id == 9).IsActive = response.Sunday;
                        }

                        BottomDescription = (response.Mode == 1) ?
                                            $"L'irrigazione si avvia i giorni selezionati alle ore {response.StartHour:00}:00" :
                                            string.Empty;
                    }
                    catch (Exception ex)
                    {

                    }

                    await Task.Delay(1000);
                }
            });
        }

        public async void StartWatering(IrrigationModel irrigation)
        {
            irrigation.IsBusy = true;

            await irrigationRestService.SetWatering(irrigation.Id, true);

            await Task.Delay(3000);
            irrigation.IsBusy = false;
        }

        public async void StopWatering(IrrigationModel irrigation)
        {
            await irrigationRestService.SetWatering(irrigation.Id, false);
        }

        public async void SetTimer(IrrigationModel irrigation, int minutes)
        {
            MinutesUpdateWait = DateTime.Now.AddSeconds(5);
            await irrigationRestService.SetTimer(irrigation.Id, minutes);
        }

        public async void ToggleIsActive(IO io)
        {
            io.IsBusy = true;

            switch (io.Id)
            {
                case 1: await irrigationRestService.SetMode(!io.IsActive ? 1 : 2); break;
                case 3: await irrigationRestService.SetDays(DayOfWeek.Monday, !io.IsActive); break;
                case 4: await irrigationRestService.SetDays(DayOfWeek.Tuesday, !io.IsActive); break;
                case 5: await irrigationRestService.SetDays(DayOfWeek.Wednesday, !io.IsActive); break;
                case 6: await irrigationRestService.SetDays(DayOfWeek.Thursday, !io.IsActive); break;
                case 7: await irrigationRestService.SetDays(DayOfWeek.Friday, !io.IsActive); break;
                case 8: await irrigationRestService.SetDays(DayOfWeek.Saturday, !io.IsActive); break;
                case 9: await irrigationRestService.SetDays(DayOfWeek.Sunday, !io.IsActive); break;
            }

            await Task.Delay(3000);
            io.IsBusy = false;
        }
    }
}
