using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts.Responses
{
    public class SchedulerFetchResponse
    {
        public IEnumerable<SchedulerContract> Schedulers { get; set; }
    }
}
