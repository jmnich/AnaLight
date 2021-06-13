using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using AnaLight.Models;
using AnaLight.Commands;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Defaults;

namespace AnaLight.ViewModels
{
    public class BufferViewerViewModel : TabViewModel
    {
        public enum BufferViewerMode
        {
            ARCHIVE_VIEWER,
            LIVE_VIEWER,
        }

        #region Properties with INotify interface
        #endregion // Properties with INotify interface

        #region Properties - other
        private BufferViewerModel _model;
        public BufferViewerMode ViewerMode { get; }
        public ObservableCollection<BasicSpectraContainer> Spectra { get; }
        public SeriesCollection ChartSeries;
        #endregion // Properties - other

        #region Commands
        public DisplayChartsCommand ChangeDisplayedChartsCommand { get; private set; }
        #endregion // Commands

        #region Events
        public event EventHandler<int> SpectraCountChanged;
        #endregion // Events

        public BufferViewerViewModel()
        {
            Spectra = new ObservableCollection<BasicSpectraContainer>();
            ViewerMode = BufferViewerMode.ARCHIVE_VIEWER;

            InitializeObject();
        }

        public BufferViewerViewModel(ObservableCollection<BasicSpectraContainer> spectraList = null)
        {
            Spectra = spectraList ?? new ObservableCollection<BasicSpectraContainer>();

            if (spectraList == null)
            {
                ViewerMode = BufferViewerMode.ARCHIVE_VIEWER;
            }
            else
            {
                ViewerMode = BufferViewerMode.LIVE_VIEWER;
            }

            InitializeObject();
        }

        private void InitializeObject()
        {
            _model = new BufferViewerModel(Spectra);
            ChangeDisplayedChartsCommand = new DisplayChartsCommand(OnChangeDisplayedChartsCommand);
            ChartSeries = new SeriesCollection();

            Spectra.CollectionChanged += (s, p) =>
            {
                SpectraCountChanged?.Invoke(this, Spectra.Count);
            };
        }

        private void OnChangeDisplayedChartsCommand(List<BasicSpectraContainer> spectra)
        {
            Debug.WriteLine($"Displaying {spectra.Count} spectra");

            if(ChartSeries is SeriesCollection seriesCollection)
            {
                if (seriesCollection.Count > 0)
                {
                    while (seriesCollection.Count > 0)
                    {
                        seriesCollection.RemoveAt(seriesCollection.Count - 1);   
                    }
                }

                foreach (BasicSpectraContainer spectrum in spectra)
                {
                    if (ConvertSpectrumToSeries(spectrum) is GLineSeries series)
                    {
                        seriesCollection.Add(series);
                    }
                }
            }

            Debug.WriteLine($"Chart has now {ChartSeries.Count} spectra");
        }

        private GLineSeries ConvertSpectrumToSeries(BasicSpectraContainer spectrum)
        {
            if(spectrum.XAxis.Length != spectrum.YAxis.Length)
            {
                Debug.WriteLine("Invalid spectrum!");
                return null;
            }
           
            ObservablePoint[] points = new ObservablePoint[spectrum.YAxis.Length];
            for(int i = 0; i < spectrum.YAxis.Length; i++)
            {
                points[i] = new ObservablePoint
                {
                    X = spectrum.XAxis[i],
                    Y = spectrum.YAxis[i],
                };
            }

            var ser = new GLineSeries();
            ser.Title = spectrum.Name;
            ser.Values = new GearedValues<ObservablePoint>();
            ser.Values.AddRange(points);

            return ser;
        }

        #region Model event handlers
        #endregion // Model event handlers
    }
}
