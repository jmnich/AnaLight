using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AnaLight.Factories;

namespace AnaLight
{
    public class MainWindowModel : IDevicePanelRegistrator
    {
        private MainWindow MainWindow { get; }
        private MainWindowViewModel MainWindowViewModel { get; }
        private ObservableCollection<TabBase> Tabs { get; }
        private DeviceControlPanelFactory PanelFactory { get; }

        public MainWindowModel(MainWindow window)
        {
            Tabs = new ObservableCollection<TabBase>();
            PanelFactory = new DeviceControlPanelFactory(this);

            // initialize MVVM objects for the main window
            MainWindowViewModel = new MainWindowViewModel
            {
                Tabs = this.Tabs
            };

            MainWindow = window;
            MainWindow.DataContext = MainWindowViewModel;

            // create the home panel tab
            PanelFactory.CreatePanel(DeviceControlPanelType.HOME);

            for (int i = 0; i < 3; i++)
            {
                PanelFactory.CreatePanel(DeviceControlPanelType.DUMMY);
            }
        }

        /// <summary>
        /// Handle an event created by pressing a close button on one of the tabs
        /// </summary>
        public void OnPanelCloseRequest(object sender, string header)
        {
            Debug.WriteLine($"Closing tab: {((TabBase)(sender)).TabHeaderText}");

            Tabs.Remove((TabBase)sender);
            ((TabBase)sender).TabCloseRequest -= OnPanelCloseRequest;
        }

        /// <summary>
        /// Register a new tab so that it can be navigated to through the main window
        /// </summary>
        public void RegisterNewDevicePanel(TabBase tabBase)
        {
            Tabs.Add(tabBase);
        }
    }
}
