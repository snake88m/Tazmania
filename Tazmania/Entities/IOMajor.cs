using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public enum IOMajor
    {
        /// <summary>
        /// Punto luce
        /// </summary>
        Light = 1,

        /// <summary>
        /// Comando alza tapparella
        /// </summary>
        ShutterUp,

        /// <summary>
        /// Include i comandi alza/abbassa tapparella
        /// </summary>
        Shutter,

        /// <summary>
        /// Presa elettrica
        /// </summary>
        Socket,

        /// <summary>
        /// Irrigatore
        /// </summary>
        Sprinkler,

        /// <summary>
        /// Valvola termostica riscaldamento
        /// </summary>
        Valve,

        /// <summary>
        /// Sirena esterna
        /// </summary>
        Siren,

        /// <summary>
        /// Avviso acustico interno
        /// </summary>
        InternalSiren,

        /// <summary>
        /// Combinatore telefonico
        /// </summary>
        Dialer,

        /// <summary>
        /// Ventola
        /// </summary>
        Fan,

        /// <summary>
        /// Sensore di contatto - porta
        /// </summary>
        DoorSensor = 20,

        /// <summary>
        /// Sensore di contatto - finestra apertura totale
        /// </summary>
        WindowSensor,

        /// <summary>
        /// Sensore di contatto - finestra apertura vasistas
        /// </summary>
        VasistasSensor,

        /// <summary>
        /// Pulsante antipanico
        /// </summary>
        EmergencyButton,

        /// <summary>
        /// Sensore rilevamento pioggia
        /// </summary>
        RainSensor,

        /// <summary>
        /// Sensore antifumo
        /// </summary>
        SmokeSensor,

        /// <summary>
        /// Sensore rilevamento temperatura
        /// </summary>
        TemperatureSensor = 40
    }
}
