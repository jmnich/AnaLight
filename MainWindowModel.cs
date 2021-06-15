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
    public class MainWindowModel : IPanelRegistrator
    {
        private MainWindow MainWindow { get; }
        private MainWindowViewModel MainWindowViewModel { get; }
        private ObservableCollection<TabBase> Tabs { get; }
        private PanelFactory PanelFactory { get; }

        public MainWindowModel(MainWindow window)
        {
            Tabs = new ObservableCollection<TabBase>();
            PanelFactory = new PanelFactory(this);

            // initialize MVVM objects for the main window
            MainWindowViewModel = new MainWindowViewModel
            {
                Tabs = this.Tabs
            };

            MainWindow = window;
            MainWindow.DataContext = MainWindowViewModel;

            // create the home panel tab
            PanelFactory.CreatePanel(PanelType.HOME);
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
        public void RegisterNewPanel(TabBase tabBase)
        {
            Tabs.Add(tabBase);
        }
    }
}
