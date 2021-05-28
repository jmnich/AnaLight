using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Models;

namespace AnaLight.ViewModels
{
    public class BasicLiveSpectraViewModel : TabViewModel
    {
        private BasicLiveSpectraModel _model;

        public BasicLiveSpectraViewModel()
        {
            _model = new BasicLiveSpectraModel();
        }
    }
}
