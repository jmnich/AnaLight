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
        public event EventHandler<BasicSpectraContainer> NewSpectraAvailable;
        public event EventHandler<string> AdapterError;

        //private BackgroundWorker WorkerThread { get; set; }
        private bool ContinueReceiving { get; set; }

        private bool IsConnected { get; set; }

        private byte[] singleFrameBuffer;

        private DateTime portOpenTimestamp;
        private DateTime lastDataPacketTimeStamp;

        private readonly int singleFrameBytes = 3694 * 2;

        /// <summary>
        /// If delay between two rx packets is larger than that the current buffer will be
        /// dropped and it will be assumed that new packet is arriving.
        /// </summary>
        private readonly int maximumPacketDelay = 200;

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

                portOpenTimestamp = DateTime.Now;
                lastDataPacketTimeStamp = DateTime.Now;

                _serialPort.Open();
            }
            catch(Exception)
            {
                // failed to connect
                AdapterError?.Invoke(this, "Failed to connect");
            }
        }

        public void SetStreamEnabled(bool _Enbaled)
        {
            ContinueReceiving = _Enbaled;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // read available bytes from the serial port
            var buf = new byte[_serialPort.BytesToRead];
            _serialPort.Read(buf, 0, buf.Length);

            // verify delay from the previous packet 
            if ((singleFrameBuffer.Length != 0) && 
                ((DateTime.Now - lastDataPacketTimeStamp).TotalMilliseconds >= maximumPacketDelay))
            {
                // delay breached 
                Debug.WriteLine("> Buffer content dropped due to maximum delay breach");
                singleFrameBuffer = new byte[0];
            }

            // handle received packet
            lastDataPacketTimeStamp = DateTime.Now;
            singleFrameBuffer = singleFrameBuffer.Concat(buf).ToArray();
            //Debug.WriteLine($"Time: {(DateTime.Now - portOpenTimestamp).TotalMilliseconds}\tAdded {buf.Length} bytes to buffer so now it is {singleFrameBuffer.Length} bytes long");

            if (singleFrameBuffer.Length > singleFrameBytes)
            {
                // something went wrong and there are too many bytes in the buffer
                Debug.WriteLine("> Packet dropped due to too many bytes");
                singleFrameBuffer = new byte[0];
            }
            else if (singleFrameBuffer.Length == singleFrameBytes)
            {
                // there should be a full frame in the buffer, parse it and fire and event
                UInt16[] points = new UInt16[singleFrameBytes / 2];

                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = BitConverter.ToUInt16(singleFrameBuffer, i * 2);
                }

                //Debug.WriteLine($">>> Purging {singleFrameBuffer.Length} bytes from buffer");
                singleFrameBuffer = new byte[0];

                // if receiving is enabled then parse and fire an event
                if(ContinueReceiving)
                {
                    double[] x = new double[points.Length];
                    double[] y = new double[points.Length];

                    for (int i = 0; i < points.Length; i++)
                    {
                        x[i] = (double)(i + 1);
                        y[i] = (double)(points[i]);
                    }

                    //double[] x = new double[points.Length];
                    //double[] y = new double[points.Length];

                    //for (int i = 0; i < points.Length; i++)
                    //{
                    //    x[i] = (double)(i + 1);
                    //    y[i] = (double)(points[i]);
                    //}

                    BasicSpectraContainer spectra = new BasicSpectraContainer
                    {
                        XAxis = x,
                        YAxis = y,
                        Comment = "Acquired by alpha prototype"
                    };

                    NewSpectraAvailable?.Invoke(this, spectra);
                }
            }
        }
    }
}
