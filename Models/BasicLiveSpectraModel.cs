using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AnaLight.Containers;
using AnaLight.Adapters;
using AnaLight.Factories;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;

namespace AnaLight.Models
{
    public class BasicLiveSpectraModel
    {
        public ISerialSpectraStreamerAdapter Adapter { get; private set; }

        public ObservableCollection<BasicSpectraContainer> SpectraList { get; }

        public bool SaveReceivedSpectra { get; set; }

        private readonly int _alphaPrototypeBaud = 460800;

        public event EventHandler<BasicSpectraContainer> NewSpectraReceived;
        public event EventHandler<string> PortConnected;
        public event EventHandler PortDisconnected;

        public BasicLiveSpectraModel()
        {
            Debug.WriteLine("Basic Live Spectra model created");
            SpectraList = new ObservableCollection<BasicSpectraContainer>();

            Adapter = SerialSpectraStreamerFactory.CreateSpectraStreamerAdapter(SerialStreamerAdapterType.PROTOTYPE_ALPHA);
            Adapter.AdapterError += OnAdapterError;
            Adapter.NewSpectraAvailable += OnNewSpectraAvailable;
        }

        private void OnAdapterError(object sender, string msg)
        {
            Debug.WriteLine($"Adapter error {msg}");
        }

        private void OnNewSpectraAvailable(object sender, BasicSpectraContainer spectra)
        {
            Debug.WriteLine("new spectra received");
            NewSpectraReceived?.Invoke(this, spectra);

            if(SaveReceivedSpectra)
            {
                Application.Current?.Dispatcher?.BeginInvoke(new Action(() => SpectraList?.Add(spectra)));
            }
        }

        public string[] GetListOfAvailableCOMPorts()
        {
            return SerialPort.GetPortNames(); 
        }

        public void TryConnect(string comPort)
        {
            if(SerialPort.GetPortNames().Contains(comPort))
            {
                Adapter.AttemptConnection(comPort, _alphaPrototypeBaud);

                if(Adapter.IsConnected)
                {
                    Adapter.SetStreamEnabled(true);
                    PortConnected?.Invoke(this, comPort);
                }
            }
        }

        public void Disconnect()
        {
            if(Adapter.IsConnected)
            {
                Adapter.Disconnect();

                if(!Adapter.IsConnected)
                {
                    Adapter.SetStreamEnabled(false);
                    PortDisconnected?.Invoke(this, null);
                }
            }
        }

        public void SetSpectraStreamEnabled(bool isEnabled)
        {
            if(Adapter.IsConnected)
            {
                Adapter.SetStreamEnabled(isEnabled);
            }
        }

        public double[] GetAvailableFrequencySettings()
        {
            return Adapter?.SupportedFrequencies ?? new double[0];
        }

        public List<string> GetAvailableTriggerSettings()
        {
            return Adapter?.SupportedTriggerModes ?? new List<string>();
        }

        public string GetDefaultTriggerSetting()
        {
            return Adapter?.DefaultTriggerMode ?? "";
        }

        public void SendConfigurationCommand(double frequency, int shutter)
        {
            // validate arguments
            if(!Adapter?.SupportedFrequencies.Contains(frequency) ?? true)
            {
                Debug.WriteLine($"Configuration command dropped. Incorrect frequency: {frequency}");
                return;
            }

            if(!Adapter?.SupportedShutterSettingsForFrequency(frequency).Contains(shutter) ?? true)
            {
                Debug.WriteLine($"Configuration command dropped. Incorrect shutter: {shutter}");
                return;
            }

            // pass values to the adapter
            Adapter?.ConfigureAcquisitionSettings(frequency, shutter);
        }

        public int[] GetAvailableShutterSettings(double frequency)
        {
            if(Adapter != null)
            {
                return Adapter.SupportedShutterSettingsForFrequency(frequency);
            }
            else
            {
                return new int[0];
            }
        }
    }
}
