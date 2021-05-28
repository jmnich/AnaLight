using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Adapters;

namespace AnaLight.Factories
{
    public enum SerialStreamerAdapterType
    {
        PROTOTYPE_ALPHA,
    }

    public class SerialSpectraStreamerFactory
    {
        public static ISerialSpectraStreamerAdapter CreateSpectraStreamerAdapter(SerialStreamerAdapterType type)
        {
            switch(type)
            {
                case SerialStreamerAdapterType.PROTOTYPE_ALPHA:
                    {
                        return new AlphaPrototypeAdapter();
                    }

                default:
                    return null;
            }
        }
    }
}
