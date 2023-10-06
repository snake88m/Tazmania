using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Mobile.Models
{
    public class HeatingModel : BaseModel
    {
        string targetTemperature;
        float currentTemperature;
        bool isActive;

        public int Id { get; set; }

        public string Description { get; set; }

        public string Major { get; set; }

        public string TargetTemperature
        {
            get { return targetTemperature; }
            set
            {
                targetTemperature = value;
                OnPropertyChanged(nameof(TargetTemperature));
            }
        }

        public float CurrentTemperature
        {
            get { return currentTemperature; }
            set
            {
                currentTemperature = value;
                OnPropertyChanged(nameof(CurrentTemperature));
            }
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }
}
