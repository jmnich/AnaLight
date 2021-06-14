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
using LiveCharts;
using LiveCharts.Dtos;
using System.IO;

namespace AnaLight.Views
{
    /// <summary>
    /// Interaction logic for BufferViewerView.xaml
    /// </summary>
    public partial class BufferViewerView : TabBase
    {
        private LinkedList<UIElement> cursorObjects;

        public BufferViewerView() : base("Viewer")
        {
            InitializeComponent();
            btnSaveCsv.IsEnabled = false;
            ClearSpectrumInfoBoxes();
            SetSpectrumInfoBoxesLocked(true);

            // whenever spectra are selected send command to load them to the chart area
            bufferContentList.SelectionChanged += (s, p) =>
            {
                ClearCursorObjects();

                if (bufferContentList.SelectedItems.Count <= 5 && bufferContentList.SelectedItems.Count > 0)
                {
                    List<BasicSpectraContainer> items = bufferContentList.SelectedItems.Cast<BasicSpectraContainer>().ToList();

                    if (DataContext is BufferViewerViewModel viewModel)
                    {
                        if (viewModel.ChangeDisplayedChartsCommand?.CanExecute(null) ?? false)
                        {
                            viewModel.ChangeDisplayedChartsCommand.Execute(items);
                        }
                    }

                    btnSaveCsv.IsEnabled = true;
                }
                else
                {
                    bufferContentList.UnselectAll();
                    btnSaveCsv.IsEnabled = false;
                }

                if(bufferContentList.SelectedItems.Count == 1)
                {
                    SetSpectrumInfoBoxesLocked(false);

                    var item = bufferContentList.SelectedItems[0] as BasicSpectraContainer;
                    txtSpectrumName.Text = item?.Name ?? "---Error---";
                    txtSpectrumComment.Text = item?.Comment ?? "---Error---";
                    txtSpectrumSource.Text = item?.SourceName ?? "---Error---";
                }
                else
                {
                    ClearSpectrumInfoBoxes();
                    SetSpectrumInfoBoxesLocked(true);
                }
            };

            chartSpectrum.DataClick += OnChartDataClick;

            cursorObjects = new LinkedList<UIElement>();

            (chartSpectrum.Content as Canvas).SizeChanged += (o, p) =>
            {
                ClearCursorObjects();
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

        public void OnChartDataClick(object sender, ChartPoint chartPoint)
        {
            // FIXME - cursor was annoyingly bad so I disabled it
            // ... do I even need the cursor...?

            //ClearCursorObjects();

            //// draw the stupid cursor with coordinates
            //double corr = 33;

            //CorePoint p = chartPoint.ChartLocation;
            //cursorObjects.AddLast(new Line
            //{
            //    X1 = p.X + chartSpectrum.ChartLegend.ActualWidth + corr,
            //    X2 = p.X + chartSpectrum.ChartLegend.ActualWidth + corr,
            //    Y1 = (chartSpectrum.Content as Canvas).ActualHeight - 20,
            //    Y2 = 20,
            //    Stroke = Brushes.LightGreen,
            //    StrokeThickness = 3,
            //    Opacity = 0.6,
            //});

            //cursorObjects.AddLast(new TextBlock
            //{
            //    Text = $"X: {chartPoint.X}\nY: {chartPoint.Y}",
            //    Foreground = Brushes.White,
            //    FontWeight = FontWeights.DemiBold,
            //    FontSize = 18,
            //});

            //// make sure coordinates will never be clipped by canvas boundary
            //double textYCorrection = p.Y < 50 ? -20 : 40;
            //double textXCorrection = p.X > ((chartSpectrum.Content as Canvas).ActualWidth - 250) ? -100 : 5;

            //Canvas.SetLeft(cursorObjects.Last.Value, 
            //    p.X + chartSpectrum.ChartLegend.ActualWidth + corr + textXCorrection);          
            //Canvas.SetTop(cursorObjects.Last.Value, p.Y - textYCorrection);

            //foreach (UIElement e in cursorObjects)
            //{
            //    (chartSpectrum.Content as Canvas).Children.Add(e);
            //}
        }

        private void ClearCursorObjects()
        {
            foreach (UIElement l in cursorObjects)
            {
                (chartSpectrum.Content as Canvas).Children.Remove(l);
            }
            cursorObjects.Clear();
        }

        private void Axis_RangeChanged(LiveCharts.Events.RangeChangedEventArgs eventArgs)
        {
            ClearCursorObjects();
        }

        private void Axis_RangeChanged_1(LiveCharts.Events.RangeChangedEventArgs eventArgs)
        {
            ClearCursorObjects();
        }

        private void autoScaleBtn_Click(object sender, RoutedEventArgs e)
        {
            chartSpectrum.AxisX[0].MinValue = double.NaN;
            chartSpectrum.AxisX[0].MaxValue = double.NaN;
            chartSpectrum.AxisY[0].MinValue = double.NaN;
            chartSpectrum.AxisY[0].MaxValue = double.NaN;
        }

        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void eraseBufferBtn_Click(object sender, RoutedEventArgs e)
        {
            // when erase button is clicked don't erase the buffer immediately 
            // ask user for confirmation first
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Erase Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var vm = DataContext as BufferViewerViewModel;
                if(vm?.EraseBufferCommand?.CanExecute(null) ?? false)
                {
                    vm.EraseBufferCommand.Execute(null);
                }
            }
        }

        private void btnSaveCsv_Click(object sender, RoutedEventArgs e)
        {
            if (bufferContentList.SelectedItems.Count <= 5 && bufferContentList.SelectedItems.Count > 0)
            {
                List<BasicSpectraContainer> items = bufferContentList.SelectedItems.Cast<BasicSpectraContainer>().ToList();

                if (DataContext is BufferViewerViewModel viewModel)
                {
                    if (viewModel.SaveSpectraToCSVCommand?.CanExecute(null) ?? false)
                    {
                        viewModel.SaveSpectraToCSVCommand.Execute(items);
                    }
                }
            }
        }

        private void btnSavePicture_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BufferViewerViewModel viewModel)
            {
                if (viewModel.SaveChartImageCommand?.CanExecute(null) ?? false)
                {
                    viewModel.SaveChartImageCommand.Execute(chartSpectrum);
                }
            }
        }

        private void btnSaveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (bufferContentList.SelectedItems.Count <= 5 && bufferContentList.SelectedItems.Count > 0)
            {
                List<BasicSpectraContainer> items = bufferContentList.SelectedItems.Cast<BasicSpectraContainer>().ToList();

                if (DataContext is BufferViewerViewModel viewModel)
                {
                    if (viewModel.SaveSelectedSpectraToArchiveCommand?.CanExecute(null) ?? false)
                    {
                        viewModel.SaveSelectedSpectraToArchiveCommand.Execute(items);
                    }
                }
            }
        }

        private void eraseSelectedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bufferContentList.SelectedItems.Count <= 5 && bufferContentList.SelectedItems.Count > 0)
            {
                List<BasicSpectraContainer> items = bufferContentList.SelectedItems.Cast<BasicSpectraContainer>().ToList();

                if (DataContext is BufferViewerViewModel viewModel)
                {
                    if (viewModel.EraseSelectedSpectraCommand?.CanExecute(null) ?? false)
                    {
                        viewModel.EraseSelectedSpectraCommand.Execute(items);
                    }
                }
            }
        }

        private void bufferContentList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (bufferContentList.SelectedItems.Count <= 5 && bufferContentList.SelectedItems.Count > 0)
                {
                    List<BasicSpectraContainer> items = bufferContentList.SelectedItems.Cast<BasicSpectraContainer>().ToList();

                    if (DataContext is BufferViewerViewModel viewModel)
                    {
                        if (viewModel.EraseSelectedSpectraCommand?.CanExecute(null) ?? false)
                        {
                            viewModel.EraseSelectedSpectraCommand.Execute(items);
                        }
                    }
                }
            }
        }

        private void ClearSpectrumInfoBoxes()
        {
            txtSpectrumName.Text = "";
            txtSpectrumComment.Text = "";
            txtSpectrumSource.Text = "";
        }

        private void SetSpectrumInfoBoxesLocked(bool locked)
        {
            txtSpectrumName.IsReadOnly = locked;
            txtSpectrumComment.IsReadOnly = locked;
            btnSaveChanges.IsEnabled = !locked;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (bufferContentList.SelectedItems.Count == 1)
            {
                var item = bufferContentList.SelectedItems[0] as BasicSpectraContainer;
                item.Name = txtSpectrumName.Text;
                item.Comment = txtSpectrumComment.Text;

                bufferContentList.Items.Refresh();
            }
        }
    }
}
