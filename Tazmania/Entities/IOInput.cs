using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class IOInput
    {
        // Datetime inizio evento (per esempio la pressione su un pulsante)
        public DateTime StartAction { get; set; }

        // Datetime fine evento (per esempio il rilascio di un pulsante)
        public DateTime EndAction { get; set; }

        // Indica se la logica applicativa ha già gestito il cambio di stato
        // di default i pulsanti sono stati gestiti
        public bool Handled { get; set; } = true;

        [NotMapped]
        public double TotalAction
        {
            get
            {
                if (EndAction == DateTime.MinValue)
                {
                    // l'azione è ancora in corso. restituisco il tempo trascorso fino ad ora
                    return Math.Round(DateTime.Now.Subtract(StartAction).TotalSeconds, 1);
                }
                else
                {
                    // l'azione è terminata. restituisco il tempo totale 
                    return Math.Round(EndAction.Subtract(StartAction).TotalSeconds, 1);
                }

                //// arrotondo a una cifra decimale
                //double seconds = Math.Round(EndAction.Subtract(StartAction).TotalSeconds, 1);

                //// se il valore è negativo vuol dire che l'azione non è mai iniziata o è ancora in corso
                //// per comodità restituisco 0
                //return seconds > 0 ? seconds : 0;
            }
        }
    }
}
