using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class HeatingSetting
    {
        [Key]
        public int Id { get; set; }

        public HeatingMode Mode { get; set; }

        /// <summary>
        /// Variazione minima della temperatura per avviare il riscaldamento
        /// </summary>
        public float Offset { get; set; }

        public virtual ICollection<HeatingTime> Times { get; set; } = null!;

        /// <summary>
        /// Verifica se il riscaldamento deve partire ora
        /// </summary>
        [NotMapped]
        public bool IsStarted
        {
            get
            {
                return Mode == HeatingMode.Manual ||
                       (Mode == HeatingMode.Auto && 
                        Times.Any(r => r.StartTime <= DateTime.Now.TimeOfDay && r.EndTime > DateTime.Now.TimeOfDay));
            }
        }
    }
}
