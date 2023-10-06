using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts.Responses
{
    public class IrrigationFetchResponse
    {
        public IEnumerable<IrrigationContract> Irrigations { get; set; } = null!;

        public int Mode { get; set; }

        public int StartHour { get; set; }

        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

        public bool Saturday { get; set; }

        public bool Sunday { get; set; }

        public bool RainSensor { get; set; }

    }
}
