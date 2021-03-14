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

        public String DeviceDescription { get; set; }
        public PhysicalDeviceType DeviceType { get; set; }
    }
}
