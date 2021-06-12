using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using AnaLight.Models;

namespace AnaLight.ViewModels
{
    public class BufferViewerViewModel : TabViewModel
    {
        public enum BufferViewerMode
        {
            ARCHIVE_VIEWER,
            LIVE_VIEWER,
        }

        #region Properties with INotify interface
        #endregion // Properties with INotify interface

        #region Properties - other
        private BufferViewerModel _model;
        public BufferViewerMode ViewerMode { get; }
        public ObservableCollection<BasicSpectraContainer> Spectra { get; }
        #endregion // Properties - other

        #region Commands
        #endregion // Commands

        public BufferViewerViewModel()
        {
            Spectra = new ObservableCollection<BasicSpectraContainer>();
            ViewerMode = BufferViewerMode.ARCHIVE_VIEWER;
            _model = new BufferViewerModel(Spectra);
        }

        public BufferViewerViewModel(ObservableCollection<BasicSpectraContainer> spectraList = null)
        {
            Spectra = spectraList ?? new ObservableCollection<BasicSpectraContainer>();

            if (spectraList == null)
            {
                ViewerMode = BufferViewerMode.ARCHIVE_VIEWER;
            }
            else
            {
                ViewerMode = BufferViewerMode.LIVE_VIEWER;
            }

            _model = new BufferViewerModel(Spectra);
        }

        #region Model event handlers
        #endregion // Model event handlers
    }
}
