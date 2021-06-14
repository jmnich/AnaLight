using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaLight.Tools
{
    public sealed class StringWriterUTF8 : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
