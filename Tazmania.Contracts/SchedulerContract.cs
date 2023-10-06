using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts
{
    public class SchedulerContract
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int Mode { get; set; }
    }
}
