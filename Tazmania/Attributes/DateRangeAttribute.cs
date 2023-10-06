using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Attributes
{
    public class DateRangeAttribute : RangeAttribute
    {
        public DateRangeAttribute() : base (typeof(DateTime), "01/01/2000", "31/12/2050") { }
    }
}
