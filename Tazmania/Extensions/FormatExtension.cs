using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Extensions
{
    public static class FormatExtension
    {
        public static string AsString(this DateTime value)
        {
            return value.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static string AsDateString(this DateTime value)
        {
            return value.ToString("dd-MM-yyyy");
        }

        public static string AsTimeString(this DateTime value)
        {
            return value.ToString("HH:mm:ss");
        }

        public static string AsString(this bool value)
        {
            return value ? "on" : "off";
        }
    }
}
