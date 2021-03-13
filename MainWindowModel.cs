using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AnaLight
{
    public class MainWindowModel
    {
        private MainWindow MainWindow { get; }
        private MainWindowViewModel MainWindowViewModel { get; }
        private ObservableCollection<TabBase> Tabs { get; }

        public MainWindowModel(MainWindow window)
        {
            Tabs = new ObservableCollection<TabBase>();

            for(int i = 0; i < 20; i++)
            {
                var newView = new Views.TabBaseExperiment
                {
                    TabHeaderText = $"Tab: {i + 1}",
                    TabInfo = $"Index of this tab: {i}"
                };

                newView.DataContext = new ViewModels.TabViewModel
                {
                    TabID = $"Tab: {i + 1}"
                };

                Tabs.Add(newView);
            }


            foreach(var tab in Tabs)
            {
                tab.TabCloseRequest += OnTabCloseRequest;
            }

            MainWindowViewModel = new MainWindowViewModel
            {
                Tabs = this.Tabs
            };

            MainWindow = window;
            MainWindow.DataContext = MainWindowViewModel;
        }

        public void OnTabCloseRequest(object sender, string header)
        {
            Debug.WriteLine($"Closing tab: {((TabBase)(sender)).TabHeaderText}");

            Tabs.Remove((TabBase)sender);
            ((TabBase)sender).TabCloseRequest -= OnTabCloseRequest;
        }
    }
}
