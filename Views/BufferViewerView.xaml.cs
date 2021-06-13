﻿using System;
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
                }
                else
                {
                    bufferContentList.UnselectAll();
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
    }
}
