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
using AnaLight.ViewModels;
using System.Diagnostics;

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

                if(freqSelection == null || shutterSelection == null)
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

                if(canExecute)
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
