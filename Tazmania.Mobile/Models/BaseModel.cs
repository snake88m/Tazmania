using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Mobile.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        bool isBusy;

        public bool IsBusy
        {
            get { return isBusy || IsReadOnly; }
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        // Se impostato a true forza IsBusy a essere sempre true
        public bool IsReadOnly { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
