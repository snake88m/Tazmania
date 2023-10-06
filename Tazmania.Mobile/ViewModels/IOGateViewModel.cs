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
    public class IOGateViewModel : BaseViewModel
    {
        readonly IORestService ioRestService;

        public ObservableCollection<IOGrouped> IOGroupeds { get; set; } = new();

        public IOGateViewModel(IORestService iORestService)
        {
            this.ioRestService = iORestService;

            _ = Task.Run(async () =>
            {
                await this.ioRestService.WaitWhenIsReady();

                while (true)
                {
                    try
                    {
                        var response = await this.ioRestService.GateFetchs(true);

                        if (!response.Groups.SelectMany(r => r.IOs).Select(h => h.Id).Order().SequenceEqual(IOGroupeds.SelectMany(r => r).Select(s => s.Id).Order()))
                        {
                            IOGroupeds.Clear();

                            foreach (var groupContract in response.Groups.OrderBy(r => r.Order))
                            {
                                List<IO> ios = groupContract.IOs.Select(r => new IO()
                                {
                                    Id = r.Id,
                                    Description = r.Description,
                                    GroupName = r.GroupName,
                                    IsActive = r.IsActive,
                                    Major = r.Major,
                                    Type = r.Type
                                }).OrderBy(r => r.Major).ToList();

                                IOGroupeds.Add(new IOGrouped(groupContract.Name, ios));
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
    }
}
