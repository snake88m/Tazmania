using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Contracts.Requests;
using Tazmania.Entities;
using Tazmania.Helpers;
using Tazmania.Interfaces.Automation;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;

namespace Tazmania.Services
{
    /// <summary>
    /// Espone i metodi per accedere al database in sicurezza.
    /// Sono esposte solo le operazioni per la lettura e l'inserimento di richieste
    /// </summary>
    public class DatabankService : IDatabankService
    {
        readonly IIORepository IORepository;
        readonly IRequestRepository RequestRepository;
        readonly IHeatingRepository HeatingRepository;
        readonly IIrrigationRepository IrrigationRepository;
        readonly ISchedulerRepository SchedulerRepository;
        readonly ISecurityRepository SecurityRepository;
        readonly IUserRepository UserRepository;

        public DatabankService(IIORepository ioRepository,
                               IRequestRepository requestRepository,
                               IHeatingRepository heatingRepository,
                               IIrrigationRepository irrigationRepository,
                               ISchedulerRepository schedulerRepository,
                               ISecurityRepository securityRepository,
                               IUserRepository userRepository)
        {
            IORepository = ioRepository;
            RequestRepository = requestRepository;
            HeatingRepository = heatingRepository;
            IrrigationRepository = irrigationRepository;
            SchedulerRepository = schedulerRepository;
            SecurityRepository = securityRepository;
            UserRepository = userRepository;
        }

        public async Task<IEnumerable<Heating>> FetchHeatings()
        {
            return await HeatingRepository.Fetchs();
        }

        public async Task<HeatingSetting> FetchHeatingSettings()
        {
            return await HeatingRepository.FetchSettings();
        }

        public async Task<IEnumerable<Irrigation>> FetchIrrigations()
        {
            return await IrrigationRepository.Fetchs();
        }

        public async Task<IrrigationSetting> FetchIrrigationSettings()
        {
            return await IrrigationRepository.FetchSettings();
        }

        public async Task<IEnumerable<Scheduler>> FetchSchedulers()
        {
            return await SchedulerRepository.Fetchs();
        }

        public async Task<IEnumerable<IO>> FetchIOs(params IOMajor[] majors)
        {
            return await IORepository.Fetchs(majors);
        }

        public async Task<SecuritySetting> FetchSecuritySettings()
        {
            return await SecurityRepository.FetchSettings();
        }

        public async Task SetRequest(RequestType type, int entityId, bool isActive = false, float value = 0)
        {
            await RequestRepository.Add(new Request()
            {
                Type = type,
                EntityId = entityId,
                IsActive = isActive,
                Value = value,
            });

            await RequestRepository.SaveChangesAsync();
        }

        public async Task<User?> FetchUser(string mail, string password)
        {
            return await UserRepository.Fetch(mail, SecurityHelper.GenerateSHA256(password));
        }
    }
}
