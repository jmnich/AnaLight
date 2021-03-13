using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnaLight.Commands;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using AnaLight.ViewModels;

namespace AnaLight
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Commands
        /// <summary>
        /// TODO - not sure if I need it. Delete this in future.
        /// </summary>
        private ICommand newTabCommand;
        public ICommand NewTabCommand
        {
            get
            {
                return newTabCommand;
            }

            set
            {
                newTabCommand = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Called to switch a tab displayed in the frame.
        /// </summary>
        private ICommand switchTabsCommand;
        public ICommand SwitchTabsCommand
        {
            get
            {
                return switchTabsCommand;
            }

            set
            {
                switchTabsCommand = value;
                OnPropertyChanged();
            }
        }
        #endregion // Commands

        #region Properties
        private ObservableCollection<TabBase> tabs;
        public ObservableCollection<TabBase> Tabs
        {
            get
            {
                return tabs;
            }

            set
            {
                tabs = value;
                OnPropertyChanged();
            }
        }

        private Page selectedTab;
        public Page SelectedTab
        {
            get
            {
                return selectedTab;
            }

            set
            {
                selectedTab = value;
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region Events
        //public EventHandler<TabBase> TabCloseRequest;
        //public EventHandler<TabBase> SelectedTabChanged;
        #endregion // Events

        #region Constructor
        public MainWindowViewModel()
        {
            NewTabCommand = new UniversalCommand(() => NewTab());
            SwitchTabsCommand = new ChangeSelectedTabCommand(SwitchTabs);
        }
        #endregion // Constructor

        #region PrivateMethods

        private void NewTab()
        {
            Debug.WriteLine("ae");
        }

        private void SwitchTabs(TabBase newSelectedTab)
        {
            SelectedTab = newSelectedTab;
        }
        #endregion // PrivateMethods

    }
}
