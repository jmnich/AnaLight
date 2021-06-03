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
        #region Properties with INotify interface

        private bool _comSelectionComboEnabled;
        public bool COMSelectionComboEnabled
        {
            set
            {
                _comSelectionComboEnabled = value;
                DisconnectButtonEnabled = !value;
                OnPropertyChanged();
            }
            get
            {
                return _comSelectionComboEnabled;
            }
        }

        private bool _disconnectButtonEnabled;
        public bool DisconnectButtonEnabled
        {
            set
            {
                _disconnectButtonEnabled = value;
                OnPropertyChanged();
            }
            get
            {
                return _disconnectButtonEnabled;
            }
        }

        private bool _acquisitionFrozen;
        public bool AcquisitionFrozen
        {
            set
            {
                _acquisitionFrozen = value;
                OnPropertyChanged();
            }
            get
            {
                return _acquisitionFrozen;
            }
        }

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
        #endregion // Properties with INotify interface


        private readonly BasicLiveSpectraModel _model;

        public ObservableCollection<string> Ports { get; }

        public UniversalCommand RefreshPortsCommand { get; }
        public ConnectToPortCommand ConnectToCOMCommand { get; }
        public UniversalCommand DisconnectPortCommand { get; }
        public UniversalCommand FreezeStreamSwitchCommand { get; }

        public BasicLiveSpectraViewModel()
        {
            ChartValues = new GearedValues<ObservablePoint>();
            ChartValues.WithQuality(Quality.Medium);

            _model = new BasicLiveSpectraModel();
            _model.NewSpectraReceived += OnNewSpectraReceived;
            _model.PortConnected += OnConnectedToPort;
            _model.PortDisconnected += OnDisconnectedFromPort;

            RefreshPortsCommand = new UniversalCommand(OnRefreshPortsCommand);
            ConnectToCOMCommand = new ConnectToPortCommand(OnConnectToCOMCommand);
            DisconnectPortCommand = new UniversalCommand(OnDisconnectPortCommand);
            FreezeStreamSwitchCommand = new UniversalCommand(OnAcquisitionFreezeSwitchCommand);

            Ports = new ObservableCollection<string>();

            COMSelectionComboEnabled = true;
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

        private void OnAcquisitionFreezeSwitchCommand()
        {
            AcquisitionFrozen = !AcquisitionFrozen;
            _model.SetSpectraStreamEnabled(!AcquisitionFrozen);
        }

        private void OnDisconnectPortCommand()
        {
            _model.Disconnect();
        }

        private void OnConnectToCOMCommand(string port)
        {
            _model.TryConnect(port);
        }

        private void OnConnectedToPort(object sender, string port)
        {
            COMSelectionComboEnabled = false;
        }

        private void OnDisconnectedFromPort(object sender, object arg)
        {
            COMSelectionComboEnabled = true;
            AcquisitionFrozen = false;
        }
    }
}
