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

        void SetStreamEnabled(bool _Enbaled);
        void AttemptConnection(string _ComPort, int _Baud);
        void Disconnect();
        event EventHandler<BasicSpectraContainer> NewSpectraAvailable;
        event EventHandler<string> AdapterError;
    }
}
