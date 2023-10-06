using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;

namespace Tazmania.Mobile.Services
{
    public class HeatingRestService : BaseRestService
    {
        public HeatingRestService(AuthenticationService authService) : base(authService) { }

        public async Task<HeatingFetchResponse> Fetchs(bool fullData)
        {
            return await SendPost<HeatingFetchRequest, HeatingFetchResponse>("Heating/Fetchs", new HeatingFetchRequest() { FullData = fullData });
        }

        public async Task SetTemperature(int id, float value)
        {
            await SendPost<RequestSetRequest>("Heating/Temperature/Set", new RequestSetRequest() { EntityId = id, Value = value });
        }

        public async Task SetMode(int mode)
        {
            await SendPost<RequestSetRequest>("Heating/Mode/Set", new RequestSetRequest() { Value = mode });
        }
    }
}
