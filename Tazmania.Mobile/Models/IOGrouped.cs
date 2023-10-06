using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Mobile.Models
{
    public class IOGrouped : List<IO>
    {
        public string Name { get; set; }

        public IOGrouped(string name, List<IO> ios) : base(ios)
        {
            Name = name;
        }
    }
}
