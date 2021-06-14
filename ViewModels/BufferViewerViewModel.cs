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
using System.Media;
using System.Windows.Media;
using Microsoft.Win32;
using WK.Libraries.BetterFolderBrowserNS;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
        public SpectraListCommand ChangeDisplayedChartsCommand { get; private set; }
        public SpectraListCommand SaveSpectraToCSVCommand { get; private set; }
        public UniversalCommand EraseBufferCommand { get; private set; }
        public SavePictureCommand SaveChartImageCommand { get; private set; }
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
            ChangeDisplayedChartsCommand = new SpectraListCommand(OnChangeDisplayedChartsCommand);
            SaveSpectraToCSVCommand = new SpectraListCommand(OnSaveSpectraToCSVCommand);
            EraseBufferCommand = new UniversalCommand(OnEraseBufferCommand);
            SaveChartImageCommand = new SavePictureCommand(OnSaveChartToImageCommand);
            ChartSeries = new SeriesCollection();

            Spectra.CollectionChanged += (s, p) =>
            {
                SpectraCountChanged?.Invoke(this, Spectra.Count);
            };
        }

        private void OnEraseBufferCommand()
        {
            _model.EraseSpectraBuffer();
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

        private void OnSaveSpectraToCSVCommand(List<BasicSpectraContainer> spectra)
        {
            var folderBrowser = new BetterFolderBrowser
            {
                Title = "Select destination folder",
                RootFolder = "C:\\",
                Multiselect = false,
            };

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveSpectraToCSV(folderBrowser.SelectedFolder, spectra);    
            }
        }

        private void SaveSpectraToCSV(string destinationFolder, List<BasicSpectraContainer> spectra)
        {
            foreach(BasicSpectraContainer spectrum in spectra)
            {
                // remove signs from spectra name that would be invalid in a file path
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                string fileName = spectrum.Name + ".csv";
                foreach (char c in fileName)
                {
                    if (invalid.Contains(c))
                    {
                        fileName = fileName.Replace(c.ToString(), "_");
                    }
                }

                // and save
                string filePath = Path.Combine(destinationFolder, fileName);
                File.WriteAllText(filePath, BasicSpectraContainer.ConvertToCSV(spectrum));
            }
        }

        private GLineSeries ConvertSpectrumToSeries(BasicSpectraContainer spectrum)
        {
            var ser = new GearedSpectrumContainer(spectrum)
            {
                Fill = Brushes.Transparent,
                PointGeometry = null,
                StrokeThickness = 3,
                Opacity = 0.8,
                SnapsToDevicePixels = true
            };

            return ser;
        }

        private void OnSaveChartToImageCommand(FrameworkElement chart)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "png files (*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToPng(chart, saveFileDialog.FileName);
            }
        }

        private void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        #region Model event handlers
        #endregion // Model event handlers

    }
}
