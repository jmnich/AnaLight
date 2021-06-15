using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnaLight.Factories;

namespace AnaLight.Containers
{
    public class PanelInfo
    {
        public String PanelName
        {
            get
            {
                return PanelFactory.ConvertControlPanelTypeToString(PanelType);
            }
        }

        public PanelType PanelType { get; set; }
        public String PanelDescription { get; set; }
        public PhysicalDeviceInfo[] SupportedDevices { get; set; }
        public ICommand OpenPanelCommand { get; set; }
    }
}
