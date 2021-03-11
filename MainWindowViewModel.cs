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
        #region Properties

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

        #endregion // Properties

        #region Events
        public EventHandler<TabBase> TabCloseRequest;
        #endregion // Events

        #region Constructor
        public MainWindowViewModel()
        {
            NewTabCommand = new UniversalCommand(() => NewTab());
        }
        #endregion // Constructor

        #region PrivateMethods

        private void NewTab()
        {
            Debug.WriteLine("ae");
        }

        #endregion // PrivateMethods

    }
}
