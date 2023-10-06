using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Repositories
{
    public interface IHeatingRepository : IRepository
    {
        Task<IList<Heating>> Fetchs();

        Task<HeatingSetting> FetchSettings();

        Task SetMode(HeatingMode mode);

        Task SetTemperature(int heatingId, float temperature);
    }
}
