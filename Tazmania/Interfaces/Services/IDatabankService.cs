using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Repositories;

namespace Tazmania.Interfaces.Services
{
    public interface IDatabankService
    {
        Task<IEnumerable<Heating>> FetchHeatings();

        Task<HeatingSetting> FetchHeatingSettings();

        Task<IEnumerable<Irrigation>> FetchIrrigations();

        Task<IrrigationSetting> FetchIrrigationSettings();

        Task<IEnumerable<Scheduler>> FetchSchedulers();

        Task<IEnumerable<IO>> FetchIOs(params IOMajor[] majors);

        Task<SecuritySetting> FetchSecuritySettings();

        Task SetRequest(RequestType type, int entityId, bool isActive = false, float value = 0);

        Task<User?> FetchUser(string mail, string password);
    }
}
