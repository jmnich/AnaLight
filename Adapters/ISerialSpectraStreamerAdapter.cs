using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;

namespace AnaLight.Adapters
{
    public enum TriggerModes
    {
        SINGLE,
        AUTO,
        EXTERNAL,
    }

    /// <summary>
    /// This interface represents a very very basic spectrometer which streams its results through a COM 
    /// port and sends new readings from time to time.
    /// </summary>
    public interface ISerialSpectraStreamerAdapter
    {
        bool IsConnected { get; }
        double[] SupportedFrequencies { get; }
        List<string> SupportedTriggerModes { get; }
        string DefaultTriggerMode { get; }
        TriggerModes CurrentTriggerMode { get; }

        event EventHandler<BasicSpectraContainer> NewSpectraAvailable;
        event EventHandler<string> AdapterError;

        int[] SupportedShutterSettingsForFrequency(double frequency);
        void ConfigureAcquisitionSettings(double frequency, int shutterSetting);
        void ConfigureTriggerSettings(string triggerSetting);
        void TriggerOnce();
        void SetStreamEnabled(bool enabled);
        void AttemptConnection(string comPort, int baud);
        void Disconnect();
    }
}
