using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using AnaLight.Adapters;

namespace AnaLight.Models
{
    public class BasicLiveSpectraModel
    {
        private ISerialSpectraStreamerAdapter Adapter { get; set; }

        public BasicLiveSpectraModel()
        {

        }
    }
}
