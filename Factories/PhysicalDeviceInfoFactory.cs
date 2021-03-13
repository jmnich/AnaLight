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
                        SupportedInterfaces = new string[] { "USB", "Ethernet" },
                        SupportedVersions = new string[] { "1.1", "1.5" }
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
