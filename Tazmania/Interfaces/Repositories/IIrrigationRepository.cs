using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Repositories
{
    public interface IIrrigationRepository : IRepository
    {
        Task<IList<Irrigation>> Fetchs();

        Task<IrrigationSetting> FetchSettings();

        Task SetExecution();

        Task SetMode(IrrigationMode mode);

        Task SetWeekDays(DayOfWeek dayOfWeek, bool isActive);

        Task SetWatering(int irrigationId, bool isActive);

        Task SetTimer(int irrigationId, int minutes);
    }
}
