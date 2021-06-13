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
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AnaLight.Containers;
using AnaLight.ViewModels;

namespace AnaLight.Views
{
    /// <summary>
    /// Interaction logic for BufferViewerView.xaml
    /// </summary>
    public partial class BufferViewerView : TabBase
    {
        public BufferViewerView() : base("Viewer")
        {
            InitializeComponent();

            // whenever spectra are selected send command to load them to the chart area
            bufferContentList.SelectionChanged += (s, p) =>
            {
                //var items = bufferContentList.SelectedItems;
                List<BasicSpectraContainer> items = bufferContentList.SelectedItems.Cast<BasicSpectraContainer>().ToList();

                if (DataContext is BufferViewerViewModel viewModel)
                {
                    if (viewModel.ChangeDisplayedChartsCommand?.CanExecute(null) ?? false)
                    {
                        viewModel.ChangeDisplayedChartsCommand.Execute(items);
                    }
                }
            };
        }

        public void SwitchDataContext(BufferViewerViewModel newViewModel)
        {
            DataContext = newViewModel;
            chartSpectrum.Series = newViewModel.ChartSeries;
        }

        public void OnSavedSpectraCountChanged(object sender, int cnt)
        {
            TabInfo = $"{(int)cnt} frames available";
        }
    }
}
