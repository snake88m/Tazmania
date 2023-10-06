using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;

namespace Tazmania.Mobile.Services
{
    public class SecurityRestService : BaseRestService
    {
        public SecurityRestService(AuthenticationService authService) : base(authService) { }

        public async Task<SecurityFetchResponse> Fetchs()
        {
            return await SendPost<SecurityFetchRequest, SecurityFetchResponse>("Security/Fetchs", new SecurityFetchRequest());
        }

        public async Task SetAntitheftMode(int mode)
        {
            await SendPost<RequestSetRequest>("Security/Alarm/Antitheft/Set", new RequestSetRequest() { Value = mode });
        }

        public async Task ActiveSOS()
        {
            await SendPost<RequestSetRequest>("Security/Alarm/SOS/Active", new RequestSetRequest());
        }

        public async Task ActiveAntiFire()
        {
            await SendPost<RequestSetRequest>("Security/Alarm/AntiFire/Active", new RequestSetRequest());
        }

        public async Task DeactiveAlarms()
        {
            await SendPost<RequestSetRequest>("Security/Alarm/Deactive", new RequestSetRequest());
        }

        public async Task SetNotifySiren(bool isActive)
        {
            await SendPost<RequestSetRequest>("Security/Siren/Set", new RequestSetRequest() { IsActive = isActive });
        }

        public async Task SetNotifyCall(bool isActive)
        {
            await SendPost<RequestSetRequest>("Security/Call/Set", new RequestSetRequest() { IsActive = isActive });
        }
    }
}
