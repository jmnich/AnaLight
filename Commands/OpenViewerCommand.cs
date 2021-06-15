using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnaLight.Containers;
using AnaLight.Factories;

namespace AnaLight.Commands
{
    public class OpenViewerCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private ObservableCollection<BasicSpectraContainer> _spectra;
        private PanelFactory _factory;

        public OpenViewerCommand(ObservableCollection<BasicSpectraContainer> spectraList, PanelFactory panelFactory)
        {
            _spectra = spectraList;
            _factory = panelFactory;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(CanExecute(parameter))
            {
                _factory.CreatePanel(PanelType.BUFFER_VIEWER, _spectra as object);
            }
        }
    }
}
