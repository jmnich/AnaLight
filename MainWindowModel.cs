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
            Tabs = new ObservableCollection<TabBase>
            {
                new Views.TabBaseExperiment()
            };

            MainWindowViewModel = new MainWindowViewModel
            {
                Tabs = this.Tabs
            };

            MainWindow = window;
            MainWindow.DataContext = MainWindowViewModel;
        }
    }
}
