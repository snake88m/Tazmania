using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts
{
    public class HeatingContract
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public float CurrentTemperature { get; set; }

        public float TargetTemperature { get; set; }

        public bool IsActive { get; set; }
    }
}
