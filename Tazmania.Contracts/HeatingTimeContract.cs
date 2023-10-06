using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts
{
    public class HeatingTimeContract
    {
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
