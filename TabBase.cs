using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using AnaLight.Commands;

namespace AnaLight
{
    public abstract class TabBase : Page, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Text displayed in the tab header. 
        /// </summary>
        private string tabHeaderText;
        public string TabHeaderText
        {
            get
            {
                return tabHeaderText;
            }

            set
            {
                tabHeaderText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Short information with tab description displayed in the tab selection zone.
        /// </summary>
        private string tabInfo;
        public string TabInfo
        {
            get
            {
                return tabInfo;
            }

            set
            {
                tabInfo = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Small icon displayed alongside the tab header text.
        /// </summary>
        private BitmapImage tabHeaderPicture;
        public BitmapImage TabHeaderPicture
        {
            get
            {
                return tabHeaderPicture;
            }

            set
            {
                tabHeaderPicture = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Constructor
        public TabBase(string header)
        {
            TabHeaderText = header;
        }
        #endregion // Constructor

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
