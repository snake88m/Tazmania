using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts.Responses
{
    public class SecurityFetchResponse
    {
        public bool NotifySiren { get; set; }

        public bool NotifyCall { get; set; }

        public bool AntiFireActive { get; set; }

        public bool SOSActive { get; set; }

        public bool AntitheftActive { get; set; }

        public string AntiFireDetail { get; set; } = null!;

        public string SOSDetail { get; set; } = null!;

        public string AntitheftDetail { get; set; } = null!;

        public int AntitheftMode { get; set; }

        public IEnumerable<IOContract> OpenGates { get; set; } = null!;
    }
}
