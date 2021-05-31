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
using AnaLight.Commands;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AnaLight.ViewModels
{
    public class BasicLiveSpectraViewModel : TabViewModel
    {
        private readonly BasicLiveSpectraModel _model;

        public ObservableCollection<string> Ports { get; }

        public UniversalCommand RefreshPortsCommand { get; }
        public ConnectToPortCommand ConnectToCOMCommand { get; }

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

            RefreshPortsCommand = new UniversalCommand(OnRefreshPortsCommand);
            ConnectToCOMCommand = new ConnectToPortCommand(OnConnectToCOMCommand);

            Ports = new ObservableCollection<string>();
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

        private void OnRefreshPortsCommand()
        {
            Ports.Clear();
            string[] ports = _model.GetListOfAvailableCOMPorts();

            if(ports?.Length != 0)
            {
                foreach(string port in ports)
                {
                    Ports.Add(port);
                }
            }
            else
            {
                Ports.Add("No ports detected");
            }
        }

        private void OnConnectToCOMCommand(string port)
        {
            Debug.WriteLine($"Connection to {port} requested");
        }
    }
}
