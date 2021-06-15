using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaLight
{
    public interface IPanelRegistrator
    {
        void RegisterNewPanel(TabBase tabBase);
        void OnPanelCloseRequest(object sender, string header);
    }
}
