using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnaLight.Commands
{
    public class OpenPanelCommand : ICommand
    {
        private Factories.PanelFactory DeviceControlPanelFactory { get; }
        private Factories.PanelType PanelType { get; }

        public OpenPanelCommand(Factories.PanelFactory deviceControlPanelFactory, 
            Factories.PanelType panelType)
        {
            DeviceControlPanelFactory = deviceControlPanelFactory;
            PanelType = panelType;
            CanExecuteChanged?.Invoke(this, null);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (DeviceControlPanelFactory is null) return false;
            else return true;
        }

        public void Execute(object parameter)
        {
            DeviceControlPanelFactory.CreatePanel(PanelType);
        }
    }
}
