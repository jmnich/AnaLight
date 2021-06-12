using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AnaLight.Containers
{
    public class BasicSpectraContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
 
        public double[] XAxis { get; set; }     // pixel number or wavelength
        public double[] YAxis { get; set; }     // counts or intensity
        public string SourceName { get; }
        public DateTime TimeStamp { get; }

        private string name;
        public string Name
        {
            get
            {
                if (name == null) return $"{TimeStamp:dd.MM hh:mm:ss.fff}";
                else return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string comment;
        public string Comment 
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                OnPropertyChanged();
            }
        }

        public BasicSpectraContainer(string source)
        {
            TimeStamp = DateTime.Now;
            SourceName = source;
        }

        public BasicSpectraContainer(string source, DateTime timeStamp)
        {
            TimeStamp = timeStamp;
            SourceName = source;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return name;
        }
    }
}
