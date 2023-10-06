using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Contracts.Responses;
using Tazmania.Mobile.Models;
using Tazmania.Mobile.Services;

namespace Tazmania.Mobile.ViewModels
{
    public class IOViewModel : BaseViewModel
    {
        readonly IORestService ioRestService;

        public ObservableCollection<IOGrouped> IOGroupeds { get; set; } = new();

        public IOViewModel(IORestService iORestService) 
        {
            this.ioRestService = iORestService;

            _ = Task.Run(async () =>
            {
                bool fullData = true;

                await this.ioRestService.WaitWhenIsReady();

                while (true)
                {
                    try
                    {
                        var response = await this.ioRestService.Fetchs(fullData);

                        if (fullData)
                        {
                            foreach (var groupContract in response.Groups.OrderBy(r => r.Order))
                            {
                                List<IO> ios = groupContract.IOs.Select(r => new IO()
                                {
                                    Id = r.Id,
                                    Description = r.Description,
                                    GroupName = r.GroupName,
                                    IsActive = r.IsActive,
                                    Major = r.Major,
                                    ShutterUpId = r.ShutterUpId,
                                    ShutterUpIsActive = r.ShutterUpIsActive,
                                    Type = r.Type,
                                    Value = r.Value,
                                    ValueCorrected = r.ValueCorrected
                                }).OrderBy(r => r.Major).ToList();

                                IOGroupeds.Add(new IOGrouped(groupContract.Name, ios));
                           }

                            fullData = false;
                        }
                        else
                        {
                            foreach (var io in IOGroupeds.SelectMany(r => r))
                            {
                                var ioContract = response.Groups.SelectMany(r => r.IOs).Single(r => r.Id == io.Id);
                                io.Value = ioContract.Value;
                                io.ValueCorrected = ioContract.ValueCorrected;
                                io.IsActive = ioContract.IsActive;
                                io.ShutterUpIsActive = ioContract.ShutterUpIsActive;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public async void ShutterUp(IO io)
        {
            bool shutterDownIsActive = io.IsActive;

            io.IsBusy = true;
            
            await ioRestService.Set(io.Id, false);
            if (!shutterDownIsActive)
            {
                await Task.Delay(100);
                await ioRestService.Set(io.ShutterUpId, true);
            }

            await Task.Delay(3000);
            io.IsBusy = false;
        }

        public async void ShutterDown(IO io)
        {
            bool shutterUpIsActive = io.ShutterUpIsActive;

            io.IsBusy = true;
           
            await ioRestService.Set(io.ShutterUpId, false);
            if (!shutterUpIsActive)
            {
                await Task.Delay(100);
                await ioRestService.Set(io.Id, true);
            } 

            await Task.Delay(3000);
            io.IsBusy = false;
        }

        public async void ShutterStop(IO io)
        {
            io.IsBusy = true;
            await ioRestService.Set(io.ShutterUpId, false);
            await Task.Delay(100);
            await ioRestService.Set(io.Id, false);
            await Task.Delay(3000);
            io.IsBusy = false;
        }

        public async void ToggleIsActive(IO io)
        {
            io.IsBusy = true;
            await ioRestService.Set(io.Id, !io.IsActive);
            await Task.Delay(3000);
            io.IsBusy = false;
        }
    }
}
