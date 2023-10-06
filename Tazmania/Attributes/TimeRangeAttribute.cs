using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Attributes
{
    public class TimeRangeAttribute : RangeAttribute
    {
        public TimeRangeAttribute() : base(typeof(TimeSpan), "00:00", "23:59") { }
    }
}
