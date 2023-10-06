using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public enum RequestType
    {
        AutomationSetOutput = 1,
        HeatingSetMode,
        HeatingSetTemperature,
        SchedulerSetMode,
        /// <summary>
        /// Avvia immediatamente le funzioni di SOS
        /// </summary>
        SecurityActiveSOS,
        /// <summary>
        /// Avvia immediatamente l'allarme antincendio
        /// </summary>
        SecurityActiveAntiFire,
        /// <summary>
        /// Disattiva tutti gli allarmi attualmente in funzione
        /// </summary>
        SecurityDeactiveAlarms,
        /// <summary>
        /// Attiva/Disattiva la sirena esterna in caso di allarme
        /// </summary>
        SecurityNotifySirenSet,
        /// <summary>
        /// Attiva/Disattiva la chiamata automatica in caso di allarme
        /// </summary>
        SecurityNotifyCallSet,
        /// <summary>
        /// Attiva/Disattiva l'antintrusione -> modalità automatica
        /// </summary>
        SecurityAntitheftSetMode,
        /// <summary>
        /// Imposta i giorni della settimana in cui l'irrigazione si deve attivare
        /// </summary>
        IrrigationSetWeekDays,
        /// <summary>
        /// Attiva/Disattiva l'irrigazione -> modalità automatica
        /// </summary>
        IrrigationSetMode,
        /// <summary>
        /// Attiva/Disattiva l'irrigazione -> manuale
        /// </summary>
        IrrigationSetWatering,
        /// <summary>
        /// Imposta il timer di irrigazione per una singola zona
        /// </summary>
        IrrigationSetTimer
    }
}
