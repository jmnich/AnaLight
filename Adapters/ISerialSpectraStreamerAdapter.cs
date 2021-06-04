using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;

namespace AnaLight.Adapters
{
    /// <summary>
    /// This interface represents a very very basic spectrometer which streams its results through a COM 
    /// port and sends new readings from time to time.
    /// </summary>
    public interface ISerialSpectraStreamerAdapter
    {
        bool IsConnected { get; }
        double[] SupportedFrequencies { get; }

        event EventHandler<BasicSpectraContainer> NewSpectraAvailable;
        event EventHandler<string> AdapterError;

        int[] SupportedShutterSettingsForFrequency(double frequency);
        void TransmitConfigurationCommand(double frequency, int shutterSetting);
        void SetStreamEnabled(bool enabled);
        void AttemptConnection(string comPort, int baud);
        void Disconnect();

    }
}
