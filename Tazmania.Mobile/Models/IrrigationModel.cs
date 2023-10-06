using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Mobile.Models
{
    public class IrrigationModel : BaseModel
    {
        string minutes;
        bool isActive;

        public int Id { get; set; }

        public string Description { get; set; }

        public string Minutes
        {
            get { return minutes; }
            set
            {
                minutes = value;
                OnPropertyChanged(nameof(Minutes));
            }
        }

        public string Major { get; set; }

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
