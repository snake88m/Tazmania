using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class Irrigation
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; } = null!;

        public IO OutputSprinkler { get; set; } = null!;

        public int OutputSprinklerId { get; set; }

        [Range(0, int.MaxValue)]
        public int Minutes { get; set; }

        public DateTime WateringEnd { get; set; }

        [NotMapped]
        public bool IsWateringInProgress
        {
            get
            {
                return WateringEnd > DateTime.Now;
            }
        }
    }
}
