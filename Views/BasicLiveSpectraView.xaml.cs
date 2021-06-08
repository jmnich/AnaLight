using AnaLight.ViewModels;
using LiveCharts;
using LiveCharts.Defaults;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnaLight.Views
{
    /// <summary>
    /// Interaction logic for BasicLiveSpectraView.xaml
    /// </summary>
    public partial class BasicLiveSpectraView : TabBase
    {
        private Line TrackingLine { get; }
        private TextBlock TrackingText { get; }
        
        public BasicLiveSpectraView() : base("Basic live spectra")
        {
            InitializeComponent();

            // initialize tracker line
            TrackingLine = new Line
            {
                X1 = 0,
                X2 = 0,
                Y1 = 0,
                Y2 = 0,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 3,
                Opacity = 0.5,
                SnapsToDevicePixels = true,
                Visibility = Visibility.Hidden,
            };

            TrackingText = new TextBlock
            {
                Text = "---",
                Foreground = Brushes.LightGreen,
                FontSize = 18,
                Visibility = Visibility.Hidden,
            };

            TrackingLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            (chartSpectra.Content as Canvas)?.Children.Add(TrackingLine);
            (chartSpectra.Content as Canvas)?.Children.Add(TrackingText);
            //ChartCanvas.Children.Add(TrackingLine);

            // fix chart settings
            chartSpectra.AxisX[0].Separator.Step = 100;
            chartSpectra.AxisX[0].Separator.StrokeThickness = 1;
            chartSpectra.AxisX[0].Separator.Stroke = Brushes.DimGray;

            // handle value selection in the combo box with COM ports
            ComPortsCombo.SelectionChanged += (sender, param) =>
            {
                var viewModel = DataContext as BasicLiveSpectraViewModel;

                bool canExecute = viewModel?.ConnectToCOMCommand?.CanExecute(null) ?? false;

                if (canExecute && (ComPortsCombo.SelectedValue is string sel) && !sel.ToLower().Contains("no"))
                {
                    viewModel?.ConnectToCOMCommand?.Execute(sel);
                }
            };

            // handle a click on the combo box - COM list should refresh
            ComPortsCombo.DropDownOpened += (sender, param) =>
            {
                var viewModel = DataContext as BasicLiveSpectraViewModel;

                bool canExecute = viewModel?.RefreshPortsCommand?.CanExecute(null) ?? false;

                if (canExecute)
                {
                    viewModel?.RefreshPortsCommand?.Execute(null);
                }
            };

            // refresh combo with shutter settings whenever frequency setting selection is changed
            FrequencyCombo.SelectionChanged += (sender, param) =>
            {
                var viewModel = DataContext as BasicLiveSpectraViewModel;
                viewModel.ConfigurationButtonEnabled = false;

                bool canExecute = viewModel?.RefreshShutterCommand?.CanExecute(null) ?? false;

                if (canExecute && (FrequencyCombo.SelectedValue is double sel))
                {
                    viewModel?.RefreshShutterCommand?.Execute(sel);
                }

                // clear period and exposure fields whenever selected frequency is changed
                periodTextBlock.Text = "-";
                exposureTextBlock.Text = "-";
            };

            // refresh calculated period and exposure whenever a new shutter setting is selected
            ShutterCombo.SelectionChanged += (sender, param) =>
            {
                var viewModel = DataContext as BasicLiveSpectraViewModel;
                viewModel.ConfigurationButtonEnabled = false;

                var freqSelection = FrequencyCombo.SelectedItem;
                var shutterSelection = ShutterCombo.SelectedItem;

                if (freqSelection == null || shutterSelection == null)
                {
                    periodTextBlock.Text = "-";
                    exposureTextBlock.Text = "-";
                }
                else
                {
                    periodTextBlock.Text = $"Period = {1.0E3 / (double)freqSelection}ms";
                    exposureTextBlock.Text = $"Exposure = {1.0E3 / (double)freqSelection / (int)shutterSelection}ms";

                    viewModel.ConfigurationButtonEnabled = true;
                }
            };

            // redraw the tracker line each time the chart gets refreshed
            chartSpectra.UpdaterTick += (sender) =>
            {
                RedrawPeakTrackerLine();
            };

            // draw the tracker line
            TrackingEnabledCheckBox.Checked += (sender, args) =>
            {
                RedrawPeakTrackerLine();
            };

            // hide the tracker line 
            TrackingEnabledCheckBox.Unchecked += (sender, args) =>
            {
                TrackingLine.X1 = 0;
                TrackingLine.X2 = 0;
                TrackingLine.Y1 = 0;
                TrackingLine.Y2 = 0;
                TrackingLine.Visibility = Visibility.Hidden;
                TrackingText.Visibility = Visibility.Hidden;
            };
        }

        // redraw the peak tracking line
        private void RedrawPeakTrackerLine()
        {
            if(TrackingEnabledCheckBox?.IsChecked ?? false)
            {
                var viewModel = DataContext as BasicLiveSpectraViewModel;

                if(viewModel == null || viewModel.MaxPoint == null)
                {
                    TrackingLine.X1 = 0;
                    TrackingLine.X2 = 0;
                    TrackingLine.Y1 = 0;
                    TrackingLine.Y2 = 0;
                    TrackingLine.Visibility = Visibility.Hidden;
                    TrackingText.Visibility = Visibility.Hidden;
                    return;
                }

                var canv = chartSpectra.Content as Canvas;

                Canvas veryInternalCanvas = null;
                foreach (UIElement u in canv.Children)
                {
                    if (u is Canvas)
                    {
                        veryInternalCanvas = u as Canvas;
                    }
                }

                double XaxisSpanInPixels = veryInternalCanvas?.ActualWidth ?? 1;
                double xCorrection = -10;
                
                // this dum formula seems to work 
                double xPixel = (viewModel.MaxPoint.X / 3694.0) * XaxisSpanInPixels + 
                    (canv.ActualWidth - veryInternalCanvas.ActualWidth) + 
                    xCorrection;

                Canvas.SetLeft(TrackingText, xPixel - 5);
                Canvas.SetTop(TrackingText, 5);

                TrackingText.Text = $"{viewModel.MaxPoint.X} | {viewModel.MaxPoint.Y}";

                TrackingLine.X1 = xPixel;
                TrackingLine.X2 = xPixel;
                TrackingLine.Y1 = 30;
                TrackingLine.Y2 = canv.ActualHeight - 50;
                TrackingLine.Visibility = Visibility.Visible;
                TrackingText.Visibility = Visibility.Visible;
            }
            else
            {
                TrackingLine.X1 = 0;
                TrackingLine.X2 = 0;
                TrackingLine.Y1 = 0;
                TrackingLine.Y2 = 0;
                TrackingLine.Visibility = Visibility.Hidden;
                TrackingText.Visibility = Visibility.Hidden;
            }
        }

        // invoke frequency and shutter configuration command from the view model
        private void BtnConfigure_Click(object sender, RoutedEventArgs e)
        {
            var freqSelection = FrequencyCombo.SelectedItem;
            var shutterSelection = ShutterCombo.SelectedItem;

            if (freqSelection != null && shutterSelection != null)
            {
                var viewModel = DataContext as BasicLiveSpectraViewModel;
                bool canExecute = viewModel?.FreqAndShutterCommand?.CanExecute(null) ?? false;

                if (canExecute)
                {
                    viewModel?.FreqAndShutterCommand?.Execute((freqSelection, shutterSelection));
                }
            }
            else
            {
                MessageBox.Show("Select frequency and shutter settings first!");
            }
        }
    }
}
