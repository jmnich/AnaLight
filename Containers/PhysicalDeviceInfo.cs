using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Factories;

namespace AnaLight.Containers
{
    public class PhysicalDeviceInfo
    {
        public String DeviceName
        {
            get
            {
                return PhysicalDeviceInfoFactory.ConvertEnumToString(DeviceType);
            }
        }

        public PhysicalDeviceType DeviceType { get; set; }
        public String[] SupportedInterfaces { get; set; }
        public String[] SupportedVersions { get; set; }
    }
}
