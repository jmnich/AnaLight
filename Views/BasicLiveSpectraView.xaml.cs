using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Wpf;

namespace AnaLight.Views
{
    /// <summary>
    /// Interaction logic for BasicLiveSpectraView.xaml
    /// </summary>
    public partial class BasicLiveSpectraView : TabBase
    {
        public BasicLiveSpectraView() : base("Basic live spectra")
        {
            InitializeComponent();

            chartSpectra.AxisX[0].Separator.Step = 100;
            chartSpectra.AxisX[0].Separator.StrokeThickness = 1;
            chartSpectra.AxisX[0].Separator.Stroke = Brushes.DimGray;
        }
    }
}
