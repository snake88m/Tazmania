using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Interfaces.Repositories;
using Tazmania.Services;

namespace Tazmania.Interfaces.Services
{
    public interface INotifyService
    {
        void PlaySirens();

        void StopSirens();

        void PlayInternalSiren();

        void StopInternalSirens();

        void SendSOSDialer();

        void StopSOSDialer();

        void SendAutomaticDialer();

        void StopAutomaticDialer();
    }
}
