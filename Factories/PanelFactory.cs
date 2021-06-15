using System;
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
    public enum PanelType
    {
        HOME,
        BASIC_LIVE_SPECTRA,
        BUFFER_VIEWER,
    }

    public class PanelFactory
    {
        private static readonly ObservableCollection<PanelInfo> availableControlPanels = new ObservableCollection<PanelInfo>();
        private static readonly ObservableCollection<PanelInfo> availableUtilityPanels = new ObservableCollection<PanelInfo>();

        public static ObservableCollection<PanelInfo> AvailableControlPanels { get { return availableControlPanels; } }
        public static ObservableCollection<PanelInfo> AvailableUtilityPanels { get { return availableUtilityPanels; } }

        private int createdTabsCounter = 0;

        /// <summary>
        /// Each new panel created by this factory will be registered using the stored in this 
        /// property. It is a way to inform the model owning a list of current panels that a new one
        /// is being created.
        /// </summary>
        private IPanelRegistrator NewPanelRegistrator { get; }

        public PanelFactory(IPanelRegistrator devicePanelRegistrator)
        {
            NewPanelRegistrator = devicePanelRegistrator;
            AvailableControlPanels.Clear();
            AvailableUtilityPanels.Clear();

            // CONTROL PANELS
            AvailableControlPanels.Add(
                new PanelInfo
                {
                    OpenPanelCommand = new OpenPanelCommand(this, PanelType.BASIC_LIVE_SPECTRA),
                    PanelType = PanelType.BASIC_LIVE_SPECTRA,
                    PanelDescription = "Basic spectra viewer capable of interfacing with simple spectrometers " +
                        "and scroling through raw data",
                    SupportedDevices = new PhysicalDeviceInfo[]
                    {
                        PhysicalDeviceInfoFactory.GetDeviceInfo(PhysicalDeviceType.PROTOTYPE_ALPHA)
                    }
                });

            // UTILITY PANELS
            AvailableUtilityPanels.Add(
                new PanelInfo
                {
                    OpenPanelCommand = new OpenPanelCommand(this, PanelType.BUFFER_VIEWER),
                    PanelType = PanelType.BUFFER_VIEWER,
                    PanelDescription = "Spectra viewer with archivization tools",
                    SupportedDevices = new PhysicalDeviceInfo[0]
                });
        }

        public void CreatePanel(PanelType type, object arg = null)
        {
            TabBase view;

            switch(type)
            {
                case PanelType.HOME:
                    {
                        view = new Views.HomePageView
                        {
                            TabHeaderText = "Home tab",
                            TabInfo = "",
                            DataContext = new ViewModels.HomePageViewModel()
                        };

                        break;
                    }

                case PanelType.BASIC_LIVE_SPECTRA:
                    {
                        view = new Views.BasicLiveSpectraView
                        {
                            TabHeaderText = "Basic live spectra",
                            TabInfo = "Acquisition and display of raw spectra",
                            DataContext = new ViewModels.BasicLiveSpectraViewModel(this),
                        };

                        break;
                    }

                case PanelType.BUFFER_VIEWER:
                    {
                        var dataContext = new ViewModels.BufferViewerViewModel(arg as ObservableCollection<BasicSpectraContainer>);

                        view = new Views.BufferViewerView
                        {
                            TabHeaderText = arg == null ? "Viewer - archive" : "Viewer - live",
                            TabInfo = arg == null ? "Spectra archive viewer" : "Real-time spectra collection viewer",
                        };

                        (view as Views.BufferViewerView).SwitchDataContext(dataContext);

                        // link view model events to view
                        dataContext.SpectraCountChanged += (view as Views.BufferViewerView).OnSavedSpectraCountChanged;

                        break;
                    }

                default:
                    throw new Exception("Device control panel factory: unknown panel requested");
            }

            NewPanelRegistrator.RegisterNewPanel(view);
            view.TabCloseRequest += NewPanelRegistrator.OnPanelCloseRequest;
            createdTabsCounter++;
            Debug.WriteLine($"Device control panel factory created a panel: /n/t{type}/n/tWhich is a tab number {createdTabsCounter}");
        }

        public static string ConvertControlPanelTypeToString(PanelType _type)
        {
            switch (_type)
            {
                case PanelType.BASIC_LIVE_SPECTRA:
                    {
                        return "Basic live spectra";
                    }

                case PanelType.BUFFER_VIEWER:
                    {
                        return "Viewer";
                    }

                default:
                    {
                        return $"Panel - {_type}";
                    }
            }
        }
    }
}
