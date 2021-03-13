using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnaLight.Commands
{
    public class ChangeSelectedTabCommand : ICommand
    {
        #region ICommand Event
        public event EventHandler CanExecuteChanged;
        #endregion // ICommand Event

        #region Delegates
        public delegate void SwitchHandler(TabBase _selectedTab);
        #endregion // Delegates

        #region Properties
        private SwitchHandler TabSwitchHandler { get; }

        private bool isSwitchingLocked = false;
        public bool IsSwitchingLocked
        {
            get
            {
                return isSwitchingLocked;
            }

            set
            {
                isSwitchingLocked = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
        #endregion // Properties

        #region Constructor
        public ChangeSelectedTabCommand(SwitchHandler _switchHandler)
        {
            TabSwitchHandler = _switchHandler;
        }
        #endregion // Constructor

        #region ICommand Methods
        public bool CanExecute(object parameter)
        {
            return !IsSwitchingLocked;
        }

        public void Execute(object parameter)
        {
            var newSelectedTab = parameter as TabBase;

            if(CanExecute(null))
            {
                TabSwitchHandler?.Invoke(newSelectedTab);
            }
        }
        #endregion // ICommand Methods
    }
}
