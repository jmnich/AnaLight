using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AnaLight.Commands
{
    public class SavePictureCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public delegate void SavePictureDelegate(FrameworkElement visual);
        public SavePictureDelegate SavePictureHandler { get; }

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

        public SavePictureCommand(SavePictureDelegate handler)
        {
            SavePictureHandler = handler;
            CanExecuteProperty = true;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteProperty;
        }

        public void Execute(object parameter)
        {
            SavePictureHandler?.Invoke(parameter as FrameworkElement);
        }
    }
}
