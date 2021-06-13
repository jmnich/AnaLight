﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnaLight.Containers;

namespace AnaLight.Commands
{
    public class DisplayChartsCommand : ICommand
    {
        public delegate void HandlerDelegate(List<BasicSpectraContainer> spectra);

        private HandlerDelegate Handler { get; }

        public DisplayChartsCommand(HandlerDelegate handler)
        {
            Handler = handler;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(CanExecute(null))
            {
                List<BasicSpectraContainer> items = (List<BasicSpectraContainer>)parameter;
                Handler?.Invoke(items);
            }
        }
    }
}
