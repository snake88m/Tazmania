using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class MailObject
    {
        /// <summary>
        /// Template che verrà applicato nel body html della mail
        /// </summary>
        public string Template { get; set; } = string.Empty;

        /// <summary>
        /// Percorso dove sono archiviati i template
        /// Necessario perchè se usato in contesto web o desktop i path cambiano
        /// </summary>
        //public string TemplatePath { get; set; } = string.Empty;

        /// <summary>
        /// Oggetto
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Elenco dei riceventi
        /// </summary>
        //public List<string> Recipients { get; set; } = new List<string>();

        /// <summary>
        /// Elenco dei testi da sovrascrivere nel template  html
        /// </summary>
        public Dictionary<string, string> Messages { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Mail con priorità alta
        /// </summary>
        public bool Priority { get; set; }
    }
}
