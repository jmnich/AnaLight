using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace AnaLight.Adapters
{
    public class AlphaPrototypeAdapter : ISerialSpectraStreamerAdapter
    {
        public event EventHandler<BasicSpectraContainer> OnNewSpectraAvailable;
        public event EventHandler<string> OnAdapterError;

        //private BackgroundWorker WorkerThread { get; set; }
        private Thread rxThread;
        private bool ContinueReceiving { get; set; }

        private bool IsConnected { get; set; }

        private byte[] singleFrameBuffer;

        private readonly int singleFrameBytes = 3694 * 2;

        public AlphaPrototypeAdapter()
        {
            ContinueReceiving = false;
            singleFrameBuffer = new byte[0];
        }

        private SerialPort _serialPort;

        public void AttemptConnection(string _ComPort, int _Baud)
        {
            try
            {
                _serialPort = new SerialPort();
                _serialPort.PortName = _ComPort;
                _serialPort.BaudRate = _Baud;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;

                _serialPort.DtrEnable = true;
                _serialPort.DataReceived += OnDataReceived;

                _serialPort.Open();

                //rxThread.Start();
            }
            catch(Exception)
            {
                // failed to connect
                OnAdapterError?.Invoke(this, "Failed to connect");
            }
        }

        public void SetStreamEnabled(bool _Enbaled)
        {
            ContinueReceiving = _Enbaled;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var buf = new byte[_serialPort.BytesToRead];
            _serialPort.Read(buf, 0, buf.Length);

            singleFrameBuffer = singleFrameBuffer.Concat(buf).ToArray();
            Debug.WriteLine($"Added {buf.Length} bytes to buffer so now it is {singleFrameBuffer.Length} bytes long");

            if(singleFrameBuffer.Length >= singleFrameBytes)
            {
                Debug.WriteLine($">>> Purging {singleFrameBuffer.Length} bytes from buffer");
                singleFrameBuffer = new byte[0];
            }
        }
    }
}
