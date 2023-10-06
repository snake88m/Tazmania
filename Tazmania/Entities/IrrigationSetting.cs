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
    public class IrrigationSetting
    {
        [Key]
        public int Id { get; set; }

        public IrrigationMode Mode { get; set; }

        public IO RainSensor { get; set; } = null!;

        public int RainSensorId { get; set; }

        [TimeRange]
        public TimeSpan StartTime { get; set; }

        [DateRange]
        public DateTime LastExecution { get; set; }

        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

        public bool Saturday { get; set; }

        public bool Sunday { get; set; }

        /// <summary>
        /// Verifica se l'irrigazione deve partire ora
        /// </summary>
        [NotMapped]
        public bool IsStarted
        {
            get
            {
                bool[] weekDays = { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };

                return Mode == IrrigationMode.Auto &&
                       weekDays[(int)DateTime.Now.DayOfWeek] &&
                       LastExecution.Date < DateTime.Now.Date &&
                       DateTime.Now.Hour == StartTime.Hours;
            }
        }
    }
}
