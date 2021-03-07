using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<TabItem> Tabs { get; }

        public MainWindowModel(MainWindow window)
        {
            MainWindow = window;

            Tabs = new ObservableCollection<TabItem>();

            MainWindowViewModel = new MainWindowViewModel
            {
                Tabs = this.Tabs
            };

            MainWindow.DataContext = MainWindowViewModel;

            Tabs.Add(new TabItem
            {
                Header = "ae"
            });
        }
    }
}
