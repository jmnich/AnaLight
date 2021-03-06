using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnaLight.Commands
{
    public class UniversalCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public delegate void BasicHandler();
        
        public BasicHandler Handler { get; }

        public UniversalCommand(BasicHandler aHandler)
        {
            Handler = aHandler;
            CanExecuteChanged?.Invoke(this, null);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Handler?.Invoke();
        }
    }
}
