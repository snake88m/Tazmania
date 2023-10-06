using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class Heating
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; } = null!;

        public float Temperature { get; set; }

        public IO InputSensor { get; set; } = null!;

        public int InputSensorId { get; set; }

        public IO OutputValve { get; set; } = null!;

        public int OutputValveId { get; set; }
    }
}
