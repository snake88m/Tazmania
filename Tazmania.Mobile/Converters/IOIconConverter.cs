using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Mobile.Models;

namespace Tazmania.Mobile.Converters
{
    public class IOIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return "";

            string isActiveText = (bool)values[1] ? "_on" : "";

            switch (values[0].ToString().ToLower())
            {
                case "light":
                    return $"light{isActiveText}.png";
                case "dialer":
                    return $"dialer{isActiveText}.png";
                case "emergencybutton":
                    return $"emergency_button{isActiveText}.png";
                case "antithefttotal":
                    return $"antitheft_total{isActiveText}.png";
                case "antitheftpartial":
                    return $"antitheft_partial{isActiveText}.png";
                case "rainsensor":
                    return $"rain_sensor{isActiveText}.png";
                case "shutter":
                    return "shutter.png";
                case "siren":
                case "internalsiren":
                    return $"siren{isActiveText}.png";
                case "smokesensor":
                    return $"smoke_sensor{isActiveText}.png";
                case "socket":
                    return $"socket{isActiveText}.png";
                case "sprinkler":
                    return $"sprinkler{isActiveText}.png";
                case "heatingsensor":
                    return $"heating_sensor{isActiveText}.png";
                case "heatingauto":
                    return $"heating_auto{isActiveText}.png";
                case "heatingmanual":
                    return $"heating_manual{isActiveText}.png";
                case "monday":
                    return $"letter_l{isActiveText}.png";
                case "tuesday":
                case "wednesday":
                    return $"letter_m{isActiveText}.png";
                case "thursday":
                    return $"letter_g{isActiveText}.png";
                case "friday":
                    return $"letter_v{isActiveText}.png";
                case "saturday":
                    return $"letter_s{isActiveText}.png";
                case "sunday":
                    return $"letter_d{isActiveText}.png";
                case "doorsensor":
                    return $"door{isActiveText}.png";
                case "windowsensor":
                    return $"window{isActiveText}.png";
                case "vasistassensor":
                    return $"vasistas{isActiveText}.png";
                default:
                    return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
