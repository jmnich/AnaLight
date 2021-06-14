using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Geared;
using AnaLight.Containers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts.Defaults;

namespace AnaLight.Containers
{
    public class GearedSpectrumContainer : GLineSeries, INotifyPropertyChanged
    {
        public new string Name
        {
            get
            {
                return RawSpectrum.Name;
            }

            set
            {
                RawSpectrum.Name = value;
                Title = value;
                OnPropertyChanged();
            }
        }

        public string Comment
        {
            get
            {
                return RawSpectrum.Comment;
            }

            set
            {
                RawSpectrum.Comment = value;
                OnPropertyChanged();
            }
        }

        public BasicSpectraContainer RawSpectrum { get; }

        public GearedSpectrumContainer(BasicSpectraContainer spectrum)
        {
            RawSpectrum = spectrum ?? throw new ArgumentNullException();
            if (RawSpectrum.XAxis.Length != RawSpectrum.YAxis.Length) 
                throw new ArgumentException("unequal axis' point counts");
            if(RawSpectrum.XAxis.Length < 1)
                throw new ArgumentException("empty axis");

            // copying and parsing...
            Name = RawSpectrum.Name;
            Comment = RawSpectrum.Comment;

            ObservablePoint[] points = new ObservablePoint[RawSpectrum.YAxis.Length];
            for (int i = 0; i < RawSpectrum.YAxis.Length; i++)
            {
                points[i] = new ObservablePoint
                {
                    X = RawSpectrum.XAxis[i],
                    Y = RawSpectrum.YAxis[i],
                };
            }

            Values = new GearedValues<ObservablePoint>();
            (Values as GearedValues<ObservablePoint>).WithQuality(Quality.High);
            Values.AddRange(points);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
