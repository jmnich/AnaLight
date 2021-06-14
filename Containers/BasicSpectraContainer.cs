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
                if (name == null) return $"{TimeStamp:dd.MM HH:mm:ss.fff}";
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

        public static string ConvertToCSV(BasicSpectraContainer spectrum)
        {
            if (spectrum.XAxis.Length != spectrum.YAxis.Length)
                throw new ArgumentException("incorrect axis length in spectrum");
            
            var builder = new StringBuilder();

            builder.AppendLine("SEP=,");
            builder.AppendLine("X,Y");

            for(int i = 0; i < spectrum.XAxis.Length; i++)
            {
                builder.AppendLine($"{spectrum.XAxis[i]},{spectrum.YAxis[i]}");
            }

            return builder.ToString();
        }
    }
}
