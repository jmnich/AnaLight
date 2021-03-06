using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnaLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel model;

        public MainWindow()
        {
            InitializeComponent();
            model = new MainWindowModel(this);

            /*
             * Now it can safely be assumed that model populated the UI as it sets DataContext of this view in its
             * constructor.
             * This code below makes sure that Home Page is immediately presented to the user when the application starts.
             */
            if(list_Tabs.Items?.Count > 0)
            {
                list_Tabs.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Capture selectection changed event from the list of tabs and execute an ICommand from the 
        /// ViewModel, which is accessed through the DataContext property of the View, which makes 
        /// it kinda compatible with the MVVM pattern but not very nice since I implemented that 
        /// in the code-behind.
        /// </summary>
        private void list_Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"Selected index: {list_Tabs.SelectedIndex}");
      
            var viewModel = (MainWindowViewModel)DataContext;

            if (e.AddedItems.Count > 0)
            {
                var selectedTab = e.AddedItems[0] as TabBase;

                if (selectedTab != null && (viewModel?.SwitchTabsCommand?.CanExecute(selectedTab) ?? false))
                {
                    viewModel.SwitchTabsCommand?.Execute(selectedTab);
                }
            }
            else
            {
                /*
                 * This occures when user decides to close the currently selected tab and i such a case
                 * the app should react by selecting the first tab from the list. If the list is empty
                 * then just set null to the Frame.
                 */

                if(list_Tabs.Items.Count > 0)
                {
                    var firstAvailableTabFromTheList = list_Tabs.Items[0];
                    
                    if (firstAvailableTabFromTheList != null && 
                        (viewModel?.SwitchTabsCommand?.CanExecute(firstAvailableTabFromTheList) ?? false))
                    {
                        viewModel.SwitchTabsCommand?.Execute(firstAvailableTabFromTheList);
                        list_Tabs.SelectedIndex = 0;
                    }
                }
                else
                {
                    viewModel.SwitchTabsCommand?.Execute(null);
                }
            }
        }
    }
}
