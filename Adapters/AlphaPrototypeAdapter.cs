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

        private bool ContinueReceiving { get; set; }

        public bool IsConnected
        {
            get
            {
                return _serialPort?.IsOpen ?? false;
            }
        }

        public double[] SupportedFrequencies
        {
            get
            {
                return new double[] { 2.0, 1.0, 0.5, 0.2, 0.1, 0.033, 0.017 };
            }
        }

        private byte[] singleFrameBuffer;

        private DateTime _lastDataPacketTimeStamp;

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

        /// <summary>
        /// Get shutter settings available for the selected frequency.
        /// </summary>
        /// <param name="frequency">One of the fixed frequencies</param>
        /// <returns>an array of available shutter settings</returns>
        public int[] SupportedShutterSettingsForFrequency(double frequency)
        {
            switch (frequency)
            {
                case 2.0:
                case 1.0:
                case 0.5:
                    {
                        return new int[] { 1, 2, 5, 10, 50, 100 };
                    }

                case 0.2:
                case 0.1:
                case 0.033:
                case 0.017:
                    {
                        return new int[] { 1, 2, 5, 10 };
                    }

                default:
                    return new int[0];
            }
        }

        public void AttemptConnection(string comPort, int baud)
        {
            if(IsConnected)
            {
                _serialPort.Close();
            }

            try
            {
                _serialPort = new SerialPort();
                _serialPort.PortName = comPort;
                _serialPort.BaudRate = baud;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;

                _serialPort.DtrEnable = true;
                _serialPort.DataReceived += OnDataReceived;

                _lastDataPacketTimeStamp = DateTime.Now;

                _serialPort.Open();
            }
            catch(Exception)
            {
                // failed to connect
                AdapterError?.Invoke(this, "Failed to connect");
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _serialPort?.Close();
            }
        }

        public void SetStreamEnabled(bool _Enbaled)
        {
            ContinueReceiving = _Enbaled;
        }

        public void TransmitConfigurationCommand(double frequency, int shutterSetting)
        {
            if(SupportedFrequencies.Contains(frequency) && SupportedShutterSettingsForFrequency(frequency).Contains(shutterSetting))
            {
                AssembleConfigurationCommand(frequency, shutterSetting, out byte[] header, out byte[] payload);

                // transmit header
                _serialPort?.Write(header, 0, header.Length);

                // transmit paylod with delay
                Task.Delay(10).ContinueWith(_ =>
                {
                    _serialPort?.Write(payload, 0, payload.Length);
                });
            }
            else
            {
                Debug.WriteLine("Incorrect configuration requested");
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // read available bytes from the serial port
            var buf = new byte[_serialPort.BytesToRead];
            _serialPort.Read(buf, 0, buf.Length);

            // verify delay from the previous packet 
            if ((singleFrameBuffer.Length != 0) && 
                ((DateTime.Now - _lastDataPacketTimeStamp).TotalMilliseconds >= maximumPacketDelay))
            {
                // delay breached 
                Debug.WriteLine("> Buffer content dropped due to maximum delay breach");
                singleFrameBuffer = new byte[0];
            }

            // handle received packet
            _lastDataPacketTimeStamp = DateTime.Now;
            singleFrameBuffer = singleFrameBuffer.Concat(buf).ToArray();

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

        private void AssembleConfigurationCommand(double frequency, int shutter, out byte[] cmdHeader, out byte[] cmdPayload)
        {
            byte[] header = new byte[16];
            byte[] payload = new byte[8];

            UInt32 cmdId = 100;
            UInt32 payloadSize = 8;
            UInt32 payloadCRC = 0;
            UInt32 headerCRC = 0;

            // horrible parsing
            var h1 = BitConverter.GetBytes(cmdId);
            var h2 = BitConverter.GetBytes(payloadSize);
            var h3 = BitConverter.GetBytes(payloadCRC);
            var h4 = BitConverter.GetBytes(headerCRC);

            // this makes me sick
            Array.Copy(h1, 0, header, 0, 4);
            Array.Copy(h2, 0, header, 4, 4);
            Array.Copy(h3, 0, header, 8, 4);
            Array.Copy(h4, 0, header, 12, 4);

            // so terrible
            int fIndex = Array.IndexOf(SupportedFrequencies, frequency);

            var p1 = BitConverter.GetBytes(fIndex);
            var p2 = BitConverter.GetBytes(shutter);

            // puke
            Array.Copy(p1, 0, payload, 0, 4);
            Array.Copy(p2, 0, payload, 4, 4);

            cmdHeader = header;
            cmdPayload = payload;
        }
    }
}
