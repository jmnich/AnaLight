using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnaLight.Commands
{
    public class RefreshShutterSettingsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public delegate void RefreshShutterHandler(double frequency);

        public RefreshShutterHandler Handler { get; }

        private bool canExecuteProperty;
        public bool CanExecuteProperty
        {
            get
            {
                return canExecuteProperty;
            }
            set
            {
                canExecuteProperty = value;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        public RefreshShutterSettingsCommand(RefreshShutterHandler handler)
        {
            Handler = handler;
            CanExecuteProperty = true;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteProperty;
        }

        public void Execute(object parameter)
        {
            Handler?.Invoke((parameter as double?) ?? Double.NaN);
        }
    }
}
