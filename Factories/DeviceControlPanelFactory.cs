﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using System.Diagnostics;
using System.Windows.Input;
using AnaLight.Commands;
using System.Collections.ObjectModel;

namespace AnaLight.Factories
{
    public enum DeviceControlPanelType
    {
        DUMMY,
        HOME,
    }

    public class DeviceControlPanelFactory
    {
        private static readonly ObservableCollection<DeviceControlPanelInfo> availableControlPanels = new ObservableCollection<DeviceControlPanelInfo>();
        public static ObservableCollection<DeviceControlPanelInfo> AvailableControlPanels { get { return availableControlPanels; } }

        private int createdTabsCounter = 0;

        /// <summary>
        /// Each new device control panel created by this factory will be registered using the stored in this 
        /// property. It is a way to inform the model in control of a list of current panels of a new one
        /// being created.
        /// </summary>
        private IDevicePanelRegistrator NewPanelRegistrator { get; }

        public DeviceControlPanelFactory(IDevicePanelRegistrator devicePanelRegistrator)
        {
            NewPanelRegistrator = devicePanelRegistrator;
            AvailableControlPanels.Clear();

            AvailableControlPanels.Add(
                new DeviceControlPanelInfo
                {
                    OpenPanelCommand = new OpenPanelCommand(this, DeviceControlPanelType.DUMMY),
                    PanelType = DeviceControlPanelType.DUMMY,
                    PanelDescription = "Dummy panel for debugging purposes",
                    SupportedDevices = new PhysicalDeviceInfo[]
                    {
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY)
                    }
                });

            AvailableControlPanels.Add(
                new DeviceControlPanelInfo
                {
                    OpenPanelCommand = new OpenPanelCommand(this, DeviceControlPanelType.DUMMY),
                    PanelType = DeviceControlPanelType.DUMMY,
                    PanelDescription = "Very dummy dum panel",
                    SupportedDevices = new PhysicalDeviceInfo[]
                    {
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY)
                    }
                });

            Debug.WriteLine("Device control panel factory instance created");
        }

        public void CreatePanel(DeviceControlPanelType type)
        {
            TabBase view;

            switch(type)
            {
                case DeviceControlPanelType.DUMMY:
                    {
                        view = new Views.TabBaseExperiment
                        {
                            TabHeaderText = $"Dummy tab",
                            TabInfo = $"Index of this tab: {createdTabsCounter}"
                        };

                        view.DataContext = new ViewModels.TabViewModel
                        {
                            TabID = $"Tab: {createdTabsCounter}"
                        };

                        break;
                    }

                case DeviceControlPanelType.HOME:
                    {
                        view = new Views.HomePageView
                        {
                            DataContext = new ViewModels.HomePageViewModel()
                        };

                        break;
                    }
                default:
                    throw new Exception("Device control panel factory: unknown panel requested");
            }

            NewPanelRegistrator.RegisterNewDevicePanel(view);
            view.TabCloseRequest += NewPanelRegistrator.OnPanelCloseRequest;
            createdTabsCounter++;
            Debug.WriteLine($"Device control panel factory created a panel: /n/t{type}/n/tWhich is a tab number {createdTabsCounter}");
        }

        //public DeviceControlPanelInfo[] GetAvailableControlPanelTypes()
        //{
        //    var controlPanelTypes = new DeviceControlPanelInfo[]
        //    {
        //        new DeviceControlPanelInfo
        //        {
        //            OpenPanelCommand = new OpenPanelCommand(this, DeviceControlPanelType.DUMMY),
        //            PanelType = DeviceControlPanelType.DUMMY,
        //            PanelDescription = "Dummy panel for debugging purposes",
        //            SupportedDevices = new PhysicalDeviceInfo[]
        //            {
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY)
        //            }
        //        },

        //        new DeviceControlPanelInfo
        //        {
        //            OpenPanelCommand = new OpenPanelCommand(this, DeviceControlPanelType.DUMMY),
        //            PanelType = DeviceControlPanelType.DUMMY,
        //            PanelDescription = "Dummy panel for debugging purposes",
        //            SupportedDevices = new PhysicalDeviceInfo[]
        //            {
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY),
        //                PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.DUMMY)
        //            }
        //        }
        //    };

        //    return controlPanelTypes;
        //}

        public static string ConvertControlPanelTypeToString(DeviceControlPanelType _type)
        {
            return $"Panel - {_type}";
        }
    }
}
