using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class SecuritySetting
    {
        [Key]
        public int Id { get; set; }

        public bool AntiFire { get; set; }

        public SecurityAntitheftMode AntitheftMode { get; set; }

        public bool NotifySiren { get; set; }

        public bool NotifyCall { get; set; }

        public DateTime AntiFireActivationDateTime { get; set; }

        [Required, MaxLength(255)]
        public string AntiFireDetail { get; set; } = null!;

        public DateTime SOSActivationDateTime { get; set; }

        [Required, MaxLength(255)]
        public string SOSDetail { get; set; } = null!;

        public DateTime AntitheftActivationDateTime { get; set; }

        [Required, MaxLength(255)]
        public string AntitheftDetail { get; set; } = null!;

        [NotMapped]
        public bool AntiFireActive
        {
            get { return AntiFireActivationDateTime > DateTime.MinValue; }
        }

        [NotMapped]
        public bool SOSActive
        {
            get { return SOSActivationDateTime > DateTime.MinValue; }
        }

        [NotMapped]
        public bool AntitheftActive
        {
            get { return AntitheftActivationDateTime > DateTime.MinValue; }
        }
    }
}
