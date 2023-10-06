using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts
{
    public class IrrigationContract
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public int Minutes { get; set; }

        public bool IsActive { get; set; }
    }
}
