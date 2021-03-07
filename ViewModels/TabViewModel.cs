using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnaLight.Commands;

namespace AnaLight.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        /// <summary>
        /// Close tab request.
        /// </summary>
        private UniversalCommand closeTabCommand;
        public UniversalCommand CloseTabCommand
        {
            get
            {
                return closeTabCommand;
            }

            private set
            {
                closeTabCommand = value;
            }
        }
    }
}
