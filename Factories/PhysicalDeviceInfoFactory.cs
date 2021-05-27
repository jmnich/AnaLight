using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;

namespace AnaLight.Factories
{
    public enum PhysicalDeviceType
    {
        DUMMY,
        PROTOTYPE_ALPHA,
    }

    public class PhysicalDeviceInfoFactory
    {
        public static PhysicalDeviceInfo GetDeviceInfo(PhysicalDeviceType _type)
        {
            switch(_type)
            {
                case PhysicalDeviceType.DUMMY:
                    return new PhysicalDeviceInfo
                    {
                        DeviceType = _type,
                        DeviceDescription = "Dummy device for UI testing. Also a very very very very very very very long description.",
                    };

                case PhysicalDeviceType.PROTOTYPE_ALPHA:
                    return new PhysicalDeviceInfo
                    {
                        DeviceType = _type,
                        DeviceDescription = "3D printed spectrometer with a transmissive grating and refractive optics",
                    };

                default:
                    return null;
            }
        }

        public static string ConvertEnumToString(PhysicalDeviceType _type)
        {
            switch(_type)
            {
                case PhysicalDeviceType.DUMMY:
                    return "Dummy Device";

                case PhysicalDeviceType.PROTOTYPE_ALPHA:
                    return "Prototype alpha";

                default:
                    return "Unknown device type";
            }
        }
    }
}
