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

namespace AnaLight.Models
{
    public class BasicLiveSpectraModel
    {
        private ISerialSpectraStreamerAdapter Adapter { get; set; }

        private ObservableCollection<BasicSpectraContainer> SpectraList { get; }

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
            SpectraList.Add(spectra);
            NewSpectraReceived?.Invoke(this, spectra);
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
    }
}
