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

namespace AnaLight.ViewModels
{
    public class BasicLiveSpectraViewModel : TabViewModel
    {
        private readonly BasicLiveSpectraModel _model;

        public SeriesCollection SeriesCollection { get; set; }

        private GearedValues<double> chartValues;
        public GearedValues<double> ChartValues
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
            double[] dummyData = new double[100];

            for (int i = 0; i < dummyData.Length; i++)
            {
                dummyData[i] = (double)i;
            }

            ChartValues = new GearedValues<double>(dummyData);

            _model = new BasicLiveSpectraModel();
            _model.NewSpectraReceived += OnNewSpectraReceived;

        }

        private void OnNewSpectraReceived(object sender, BasicSpectraContainer newSpectra)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChartValues = new GearedValues<double>(newSpectra.YAxis);
                ChartValues.WithQuality(Quality.High);
            });
        }
    }
}
