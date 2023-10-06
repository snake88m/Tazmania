using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class IOOutput
    {
        bool _newStatus;

        // Nuovo stato che verrà impostato sul relè
        public bool NewStatus
        {
            get { return _newStatus; }
            set
            {
                _newStatus = value;
                Handled = false;
            }
        }

        // Identifica se il nuovo stato è già stato impostato
        // di default true perchè al primo avvio lo stato corretto è quello di campo
        public bool Handled { get; private set; } = true;

        public void HasHandled()
        {
            Handled = true;
        }
    }
}
