using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class SchedulerItem
    {
        [Key]
        public int Id { get; set; }

        public int OutputId { get; set; }

        public IO Output { get; set; } = null!;

        public int SchedulerId { get; set; }

        public Scheduler Scheduler { get; set; } = null!;
    }
}
