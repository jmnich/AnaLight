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
using AnaLight.Factories;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AnaLight.Adapters;
using System.Windows.Threading;

namespace AnaLight.ViewModels
{
    public class BasicLiveSpectraViewModel : TabViewModel
    {
        #region Properties with INotify interface

        private Visibility _triggerButtonVisible;
        public Visibility TriggerButtonVisible
        {
            set
            {
                _triggerButtonVisible = value;
                OnPropertyChanged();
            }
            get
            {
                return _triggerButtonVisible;
            }
        }

        private Visibility _freezeButtonVisible;
        public Visibility FreezeButtonVisible
        {
            set
            {
                _freezeButtonVisible = value;
                OnPropertyChanged();
            }
            get
            {
                return _freezeButtonVisible;
            }
        }

        private bool _triggerButtonEnabled;
        public bool TriggerButtonEnabled
        {
            get
            {
                return _triggerButtonEnabled;
            }
            set
            {
                _triggerButtonEnabled = value;
                OnPropertyChanged();
            }
        }

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

        private bool _configurationButtonEnabled;
        public bool ConfigurationButtonEnabled
        {
            set
            {
                _configurationButtonEnabled = value;
                OnPropertyChanged();
            }
            get
            {
                return _configurationButtonEnabled;
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

        private int maxPointIndex;
        public int MaxPointIndex
        {
            get
            {
                return maxPointIndex;
            }

            set
            {
                maxPointIndex = value;
                OnPropertyChanged();
            }
        }

        private ObservablePoint maxPoint;
        public ObservablePoint MaxPoint
        {
            get
            {
                return maxPoint;
            }

            set
            {
                maxPoint = value;
                OnPropertyChanged();
            }
        }

        private bool saveReceivedSpectra;
        public bool SaveReceivedSpectra
        {
            get
            {
                return saveReceivedSpectra;
            }
            set
            {
                saveReceivedSpectra = value;
                _model.SaveReceivedSpectra = value;
                OnPropertyChanged();
            }
        }
        #endregion // Properties with INotify interface

        #region Properties - other
        private readonly BasicLiveSpectraModel _model;

        public ObservableCollection<string> Ports { get; }
        public ObservableCollection<double> FrequencySettings { get; }
        public ObservableCollection<int> ShutterSettings { get; }
        public ObservableCollection<string> TriggerSettings { get; }
        public string DefaultTriggerSetting { get; }

        public ObservableCollection<BasicSpectraContainer> SavedSpectra
        {
            get
            {
                return _model?.SpectraList;
            }
        }
        #endregion // Properties - other

        #region Commands

        public UniversalCommand RefreshPortsCommand { get; }
        public StringCommand ConnectToCOMCommand { get; }
        public UniversalCommand DisconnectPortCommand { get; }
        public UniversalCommand FreezeStreamSwitchCommand { get; }
        public RefreshShutterSettingsCommand RefreshShutterCommand { get; }
        public FreqAndShutterConfigCommand FreqAndShutterCommand { get; }
        public OpenViewerCommand OpenViewerPanelCommand { get; }
        public StringCommand TriggerSettingCommand { get; }
        public UniversalCommand SingleTriggerCommand { get; }
        #endregion // Commands

        private bool _awaitingForTransmissionAfterTrigger = false;

        public BasicLiveSpectraViewModel(PanelFactory panelFactory)
        {
            ChartValues = new GearedValues<ObservablePoint>();
            ChartValues.WithQuality(Quality.Medium);

            _model = new BasicLiveSpectraModel();
            _model.NewSpectraReceived += OnNewSpectraReceived;
            _model.PortConnected += OnConnectedToPort;
            _model.PortDisconnected += OnDisconnectedFromPort;

            RefreshPortsCommand = new UniversalCommand(OnRefreshPortsCommand);
            ConnectToCOMCommand = new StringCommand(OnConnectToCOMCommand);
            DisconnectPortCommand = new UniversalCommand(OnDisconnectPortCommand);
            FreezeStreamSwitchCommand = new UniversalCommand(OnAcquisitionFreezeSwitchCommand);
            RefreshShutterCommand = new RefreshShutterSettingsCommand(OnRefreshShutterSettingsCommand);
            FreqAndShutterCommand = new FreqAndShutterConfigCommand(OnFreqAndShSettingsConfigCommand);
            OpenViewerPanelCommand = new OpenViewerCommand(SavedSpectra, panelFactory);
            TriggerSettingCommand = new StringCommand(OnTriggerSettingCommand);
            SingleTriggerCommand = new UniversalCommand(OnSingleTriggerCommand);

            Ports = new ObservableCollection<string>();
            FrequencySettings = new ObservableCollection<double>(_model.GetAvailableFrequencySettings());
            ShutterSettings = new ObservableCollection<int>();
            TriggerSettings = new ObservableCollection<string>(_model.GetAvailableTriggerSettings());
            DefaultTriggerSetting = _model.GetDefaultTriggerSetting();

            COMSelectionComboEnabled = true;
            ConfigurationButtonEnabled = false;

            // configure the interface for a single trigger mode
            FreezeButtonVisible = Visibility.Hidden;
            TriggerButtonVisible = Visibility.Visible;
            TriggerButtonEnabled = true;

            SaveReceivedSpectra = false;
        }

        #region Model event handlers

        private void OnNewSpectraReceived(object sender, BasicSpectraContainer newSpectra)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                while(ChartValues.Count < newSpectra.YAxis.Length)
                {
                    ChartValues.Add(new ObservablePoint());
                }

                MaxPoint = ChartValues[0];

                for (int i = 0; i < ChartValues.Count; i++)
                {
                    ChartValues[i].X = newSpectra.XAxis[i];
                    ChartValues[i].Y = newSpectra.YAxis[i];

                    if (newSpectra.YAxis[i] > MaxPoint.Y)
                    {
                        MaxPoint = ChartValues[i];
                        MaxPointIndex = i;
                    }
                }

                if(_awaitingForTransmissionAfterTrigger)
                {
                    _awaitingForTransmissionAfterTrigger = false;

                    var dt = new DispatcherTimer(DispatcherPriority.Send);
                    dt.Tick += (s, e) =>
                    {
                        // this seems to work better with this delay, not sure why
                        dt.Stop();
                        TriggerButtonEnabled = true;
                    };
                    dt.Interval = TimeSpan.FromMilliseconds(300);
                    dt.Start();
                }
            });
        }

        private void OnConnectedToPort(object sender, string port)
        {
            COMSelectionComboEnabled = false;
            TriggerButtonEnabled = true;
        }

        private void OnDisconnectedFromPort(object sender, object arg)
        {
            COMSelectionComboEnabled = true;
            AcquisitionFrozen = false;
            TriggerButtonEnabled = false;
        }
        #endregion // Model event handlers

        #region Command handlers
        private void OnTriggerSettingCommand(string s)
        {
            Debug.WriteLine(s);

            _model.Adapter.ConfigureTriggerSettings(s);

            if(_model.Adapter.CurrentTriggerMode == TriggerModes.AUTO)
            {
                FreezeButtonVisible = Visibility.Visible;
                TriggerButtonVisible = Visibility.Hidden;
                TriggerButtonEnabled = false;
                Debug.WriteLine("auto");
            }
            else if(_model.Adapter.CurrentTriggerMode == TriggerModes.SINGLE)
            {
                FreezeButtonVisible = Visibility.Hidden;
                TriggerButtonVisible = Visibility.Visible;
                TriggerButtonEnabled = true;
                Debug.WriteLine("single");
            }
            else
            {
                FreezeButtonVisible = Visibility.Hidden;
                TriggerButtonVisible = Visibility.Hidden;
                TriggerButtonEnabled = false;
                Debug.WriteLine("ext");
            }
        }

        private void OnSingleTriggerCommand()
        {
            if(_model.Adapter.CurrentTriggerMode == TriggerModes.SINGLE)
            {
                _model.Adapter.TriggerOnce();
                _awaitingForTransmissionAfterTrigger = true;
                TriggerButtonEnabled = false;
            }
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
            ConfigurationButtonEnabled = false;
        }

        private void OnConnectToCOMCommand(string port)
        {
            _model.TryConnect(port);
        }

        private void OnRefreshShutterSettingsCommand(double frequency)
        {
            ShutterSettings.Clear();
            
            foreach(int shutterSetting in _model.GetAvailableShutterSettings(frequency))
            {
                ShutterSettings.Add(shutterSetting);
            }
        }

        private void OnFreqAndShSettingsConfigCommand(double freq, int shutter)
        {
            Debug.WriteLine($"F {freq}   SH {shutter}");
            _model?.SendConfigurationCommand(freq, shutter);
        }
        #endregion // Command handlers
    }
}
