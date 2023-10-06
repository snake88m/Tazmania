using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts.Responses
{
    public class HeatingFetchResponse
    {
        public IEnumerable<HeatingContract> Heatings { get; set; } = null!;

        public IEnumerable<HeatingTimeContract> Times { get; set; } = null!;

        public int Mode { get; set; }

        public bool IsActive { get; set; }
    }
}
