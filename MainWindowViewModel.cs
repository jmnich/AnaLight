using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnaLight.Commands;
using System.Diagnostics;

namespace AnaLight
{
    public class MainWindowViewModel
    {
        public ICommand NewTabCommand { get; }

        public MainWindowViewModel()
        {
            NewTabCommand = new UniversalCommand(() => NewTab());
        }

        private void NewTab()
        {
            Debug.WriteLine("ae");
        }

    }
}
