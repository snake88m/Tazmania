using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Attributes;

namespace Tazmania.Entities
{
    public class Scheduler
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; } = null!;

        [DateRange]
        public DateTime Start { get; set; }

        [DateRange]
        public DateTime End { get; set; }

        public SchedulerMode Mode { get; set; }

        public virtual ICollection<SchedulerItem> Items { get; set; } = null!;

        /// <summary>
        /// Verifica se il programma deve partire ora
        /// </summary>
        [NotMapped]
        public bool IsStarted
        {
            get
            {
                if (Mode == SchedulerMode.Auto)
                {
                    bool nextStart = true;
                    DateTime nextEnd = DateTime.Now.Date.Add(End.TimeOfDay);

                    if (End.Hour < DateTime.Now.Hour)
                    {
                        nextEnd = nextEnd.AddDays(1);
                        nextStart = DateTime.Now >= DateTime.Now.Date.Add(Start.TimeOfDay);
                    }

                    return nextStart && 
                           Start.Date <= DateTime.Now.Date &&
                           End.Date > DateTime.Now.Date &&
                           DateTime.Now < nextEnd;
                }

                return Mode == SchedulerMode.AlwaysOn;
            }
        }
    }
}
