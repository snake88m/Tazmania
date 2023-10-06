using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Repositories
{
    public interface ISecurityRepository : IRepository
    {
        Task<SecuritySetting> FetchSettings();

        Task SetAntitheftMode(SecurityAntitheftMode mode);

        Task ActiveAntiFire(string detail);

        Task ActiveSOS(string detail);

        Task ActiveAntitheft(string detail);

        Task DeactiveSOS();

        Task DeactiveAntiFire();

        Task DeactiveAlarms();

        Task DeactiveAntitheft();

        Task SetNotifySiren(bool isActive);

        Task SetNotifyCall(bool isActive);
    }
}
