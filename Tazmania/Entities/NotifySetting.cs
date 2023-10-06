using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class NotifySetting
    {
        [Key]
        public int Id { get; set; }

        public IO DialerSOS { get; set; } = null!;

        public int DialerSOSId { get; set; }

        public IO DialerAutomatic { get; set; } = null!;

        public int DialerAutomaticId { get; set; }
    }
}
