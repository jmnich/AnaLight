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

        public event EventHandler<BasicSpectraContainer> NewSpectraReceived;

        public BasicLiveSpectraModel()
        {
            Debug.WriteLine("Basic Live Spectra model created");
            SpectraList = new ObservableCollection<BasicSpectraContainer>();

            Adapter = SerialSpectraStreamerFactory.CreateSpectraStreamerAdapter(SerialStreamerAdapterType.PROTOTYPE_ALPHA);
            Adapter.AdapterError += OnAdapterError;
            Adapter.NewSpectraAvailable += OnNewSpectraAvailable;
            Adapter.AttemptConnection("COM3", 460800);
            Adapter.SetStreamEnabled(true);
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
    }
}
