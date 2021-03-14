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
        DUMMY
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

                default:
                    return "Unknown device type";
            }
        }
    }
}
