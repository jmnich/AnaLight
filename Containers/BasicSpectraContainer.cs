using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaLight.Containers
{
    public class BasicSpectraContainer
    {
        public double[] XAxis { get; set; }     // pixel number or wavelength
        public double[] YAxis { get; set; }     // counts or intensity
        public string Comment { get; set; }
        public DateTime TimeStamp { get; }

        public BasicSpectraContainer()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
