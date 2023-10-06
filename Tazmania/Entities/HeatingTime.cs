using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Attributes;

namespace Tazmania.Entities
{
    public class HeatingTime
    {
        [Key]
        public int Id { get; set; }

        public int HeatingSettingId { get; set; }

        public HeatingSetting HeatingSetting { get; set; } = null!;

        [TimeRange]
        public TimeSpan StartTime { get; set; }

        [TimeRange]
        public TimeSpan EndTime { get; set; }
    }
}
