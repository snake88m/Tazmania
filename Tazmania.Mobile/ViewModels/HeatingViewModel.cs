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
    public class HeatingViewModel : BaseViewModel
    {
        readonly HeatingRestService heatingRestService;

        string bottomDescription;
        DateTime TemperaturesUpdateWait = DateTime.MinValue;

        public ObservableCollection<HeatingModel> Heatings { get; set; } = new();

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


        public HeatingViewModel(HeatingRestService heatingRestService)
        {
            this.heatingRestService = heatingRestService;

            _ = Task.Run(async () =>
            {
                bool fullData = true;

                await this.heatingRestService.WaitWhenIsReady();

                while (true)
                {
                    try
                    {
                        var response = await this.heatingRestService.Fetchs(fullData);

                        if (fullData)
                        {
                            foreach (var heating in response.Heatings)
                            {
                                Heatings.Add(new HeatingModel()
                                {
                                    Id = heating.Id,
                                    Description = heating.Description,
                                    Major = "heatingsensor",
                                });
                            }

                            Options.Add(new IO() { Id = 1, Description = "Modo automatico", Major = "heatingauto" });
                            Options.Add(new IO() { Id = 2, Description = "Modo manuale", Major = "heatingmanual" });

                            fullData = false;
                        }

                        foreach (var heating in Heatings)
                        {
                            var currentHeating = response.Heatings.Single(r => r.Id == heating.Id);

                            if (DateTime.Now > TemperaturesUpdateWait)
                            {
                                heating.TargetTemperature = $"{currentHeating.TargetTemperature} °C";
                            }

                            heating.CurrentTemperature = currentHeating.CurrentTemperature;
                            heating.IsActive = currentHeating.IsActive;
                        }

                        foreach (var option in Options)
                        {
                            Options.Single(r => r.Id == 1).IsActive = (response.Mode == 1);
                            Options.Single(r => r.Id == 2).IsActive = (response.Mode == 2);
                        }

                        if (response.Mode == 1)
                        {
                            BottomDescription = $"Il riscaldamento si avvia tutti i giorni ai seguenti orari:\n{string.Join("\n", response.Times.Select(r => $"{r.StartTime.ToString("hh\\:mm")} {r.EndTime.ToString("hh\\:mm")}"))}";
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
        }
        public async void SetTargetTemperature(HeatingModel heating, float temperature)
        {
            TemperaturesUpdateWait = DateTime.Now.AddSeconds(5);
            await heatingRestService.SetTemperature(heating.Id, temperature);
        }

        public async void ToggleIsActive(IO io)
        {
            io.IsBusy = true;

            switch (io.Id)
            {
                case 1: await heatingRestService.SetMode(!io.IsActive ? 1 : 3); break;
                case 2: await heatingRestService.SetMode(!io.IsActive ? 2 : 3); break;
            }

            await Task.Delay(3000);
            io.IsBusy = false;
        }
    }
}
