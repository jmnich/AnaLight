using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnaLight.Factories;

namespace AnaLight.Containers
{
    public class DeviceControlPanelInfo
    {
        public String PanelName
        {
            get
            {
                return DeviceControlPanelFactory.ConvertControlPanelTypeToString(PanelType);
            }
        }

        public DeviceControlPanelType PanelType { get; set; }
        public String PanelDescription { get; set; }
        public PhysicalDeviceInfo[] SupportedDevices { get; set; }
        public ICommand OpenPanelCommand { get; set; }
    }
}
