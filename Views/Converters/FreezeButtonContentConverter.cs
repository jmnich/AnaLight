using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AnaLight.Views.Converters
{
    public class FreezeButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean val = (bool)value;
            
            if(val)
            {
                return "Stopped";
            }
            else
            {
                return "Running";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;

            if(val.Equals("Stopped"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
