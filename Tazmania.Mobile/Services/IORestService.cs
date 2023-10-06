using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;

namespace Tazmania.Mobile.Services
{
    public class IORestService : BaseRestService
    {
        public IORestService(AuthenticationService authService) : base(authService) { }

        public async Task<IOFetchResponse> Fetchs(bool fullData)
        {
            return await SendPost<IOFetchRequest, IOFetchResponse>("IO/Fetchs", new IOFetchRequest() { FullData = fullData });
        }

        public async Task<IOFetchResponse> GateFetchs(bool fullData)
        {
            return await SendPost<IOFetchRequest, IOFetchResponse>("IO/Gate/Fetchs", new IOFetchRequest() { FullData = fullData });
        }

        public async Task Set(int id, bool isActive)
        {
            await SendPost<RequestSetRequest>("IO/Set", new RequestSetRequest() { EntityId = id, IsActive = isActive });
        }
    }
}
