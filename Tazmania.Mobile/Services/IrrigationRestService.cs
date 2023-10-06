using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;

namespace Tazmania.Mobile.Services
{
    public class IrrigationRestService : BaseRestService
    {
        public IrrigationRestService(AuthenticationService authService) : base(authService) { }

        public async Task<IrrigationFetchResponse> Fetchs(bool fullData)
        {
            return await SendPost<IrrigationFetchRequest, IrrigationFetchResponse>("Irrigation/Fetchs", new IrrigationFetchRequest() { FullData = fullData });
        }

        public async Task SetDays(DayOfWeek dayOfWeek, bool isActive)
        {
            await SendPost<RequestSetRequest>("Irrigation/Days/Set", new RequestSetRequest() { EntityId = (int)dayOfWeek, IsActive = isActive });
        }

        public async Task SetMode(int mode)
        {
            await SendPost<RequestSetRequest>("Irrigation/Mode/Set", new RequestSetRequest() { Value = mode });
        }

        public async Task SetWatering(int irrigationId, bool isActive)
        {
            await SendPost<RequestSetRequest>("Irrigation/Watering/Set", new RequestSetRequest() { EntityId = irrigationId, IsActive = isActive });
        }

        public async Task SetTimer(int irrigationId, int minutes)
        {
            await SendPost<RequestSetRequest>("Irrigation/Timer/Set", new RequestSetRequest() { EntityId = irrigationId, Value = minutes });
        }
    }
}
