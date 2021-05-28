using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AnaLight.Containers;
using AnaLight.Adapters;
using AnaLight.Factories;

namespace AnaLight.Models
{
    public class BasicLiveSpectraModel
    {
        private ISerialSpectraStreamerAdapter Adapter { get; set; }

        public BasicLiveSpectraModel()
        {
            Debug.WriteLine("Basic Live Spectra model created");

            Adapter = SerialSpectraStreamerFactory.CreateSpectraStreamerAdapter(SerialStreamerAdapterType.PROTOTYPE_ALPHA);
            Adapter.OnAdapterError += OnAdapterError;
            Adapter.AttemptConnection("COM3", 460800);
        }

        private void OnAdapterError(object sender, string msg)
        {
            Debug.WriteLine($"Adapter error {msg}");
        }
    }
}
