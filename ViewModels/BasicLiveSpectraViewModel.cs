using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Models;
using AnaLight.Containers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Geared;
using System.Windows;
using LiveCharts.Defaults;

namespace AnaLight.ViewModels
{
    public class BasicLiveSpectraViewModel : TabViewModel
    {
        private readonly BasicLiveSpectraModel _model;

        private GearedValues<ObservablePoint> chartValues;
        public GearedValues<ObservablePoint> ChartValues
        {
            get
            {
                return chartValues;
            }

            set
            {
                chartValues = value;
                OnPropertyChanged();
            }
        }

        public BasicLiveSpectraViewModel()
        {
            ChartValues = new GearedValues<ObservablePoint>();
            ChartValues.WithQuality(Quality.Medium);

            _model = new BasicLiveSpectraModel();
            _model.NewSpectraReceived += OnNewSpectraReceived;
        }

        private void OnNewSpectraReceived(object sender, BasicSpectraContainer newSpectra)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                while(ChartValues.Count < newSpectra.YAxis.Length)
                {
                    ChartValues.Add(new ObservablePoint());
                }

                for (int i = 0; i < ChartValues.Count; i++)
                {
                    ChartValues[i].X = newSpectra.XAxis[i];
                    ChartValues[i].Y = newSpectra.YAxis[i];
                }
            });
        }
    }
}
