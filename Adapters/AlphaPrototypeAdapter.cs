using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;

namespace AnaLight.Adapters
{
    public class AlphaPrototypeAdapter : ISerialSpectraStreamerAdapter
    {
        public event EventHandler<BasicSpectraContainer> OnNewSpectraAvailable;
        public event EventHandler<string> OnAdapterError;

        public void AttemptConnection(string _ComPort, int _Baud)
        {
            throw new NotImplementedException();
        }

        public void SetStreamEnabled(bool _Enbaled)
        {
            throw new NotImplementedException();
        }
    }
}
