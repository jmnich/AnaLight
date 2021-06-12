using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;

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
        public BufferViewerMode ViewerMode { get; }
        public ObservableCollection<BasicSpectraContainer> Spectra { get; }
        #endregion // Properties - other

        #region Commands
        #endregion // Commands

        public BufferViewerViewModel(ObservableCollection<BasicSpectraContainer> spectraList)
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
        }

        #region Model event handlers
        #endregion // Model event handlers
    }
}
