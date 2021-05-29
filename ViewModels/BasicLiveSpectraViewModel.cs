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
using System.Windows;

namespace AnaLight.ViewModels
{
    public class BasicLiveSpectraViewModel : TabViewModel
    {
        private readonly BasicLiveSpectraModel _model;

        public SeriesCollection SeriesCollection { get; set; }

        private int counter = 0;

        private BasicSpectraContainer lastReceivedSpectrum;
        public BasicSpectraContainer LastReceivedSpectrum
        {
            get
            {
                return lastReceivedSpectrum;
            }

            private set
            {
                lastReceivedSpectrum = value;
                OnPropertyChanged();
            }
        }

        public BasicLiveSpectraViewModel()
        {
            SeriesCollection = new SeriesCollection();
            SeriesCollection.Clear();

            double[] dummyData = new double[100];

            for(int i = 0; i < dummyData.Length; i++)
            {
                dummyData[i] = (double)i;
            }

            SeriesCollection.Add(new LineSeries
            {
                Title = "Waiting for data",
                Values = new ChartValues<double>(dummyData),
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,
                PointGeometry = null,
            });


            _model = new BasicLiveSpectraModel();
            _model.NewSpectraReceived += OnNewSpectraReceived;

        }

        private void OnNewSpectraReceived(object sender, BasicSpectraContainer newSpectra)
        {
            if(counter >= 50)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LastReceivedSpectrum = newSpectra;

                    var data = SeriesCollection[0] as LineSeries;

                    data.Title = newSpectra.Comment;
                    data.Values = new ChartValues<double>(newSpectra.YAxis);
                });

                counter = 0;
            }
            else
            {
                counter++;
            }
        }
    }
}
