using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;

namespace AnaLight.Factories
{
    public enum DeviceControlPanelType
    {
        DUMMY
    }

    public class DeviceControlPanelFactory
    {
        public static DeviceControlPanelInfo[] GetAvailableControlPanelTypes()
        {
            var controlPanelTypes = new DeviceControlPanelInfo[]
            {
                new DeviceControlPanelInfo
                {
                    PanelType = DeviceControlPanelType.DUMMY,
                    PanelDescription = "Dummy panel for debugging purposes",
                    SupportedDevices = new PhysicalDeviceInfo[]
                    {
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY)
                    }
                }
            };

            return controlPanelTypes;
        }

        public static string ConvertControlPanelTypeToString(DeviceControlPanelType _type)
        {
            switch (_type)
            {
                case DeviceControlPanelType.DUMMY:
                    return "Dummy control panel";

                default:
                    return "Unknown control panel";
            }
        }
    }
}
