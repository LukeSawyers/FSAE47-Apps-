using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;

namespace AccumulatorMonitorM017.Backend
{
    /// <summary>
    /// Serial layer to implement basic serial port funcitonality
    /// </summary>
    public class SerialInterface
    {
        const int BufferSize = 104;

        #region Static Members

        /// <summary>
        /// Gets the available ports for this computer
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }


        /// <summary>
        /// Collection of interfaces, referenced by their names
        /// </summary>
        public static Dictionary<string, SerialInterface> OpenInterfaces = new Dictionary<string, SerialInterface>();

        /// <summary>
        /// indicates that the static elements have been initialised
        /// </summary>
        public static bool Initialised = false;

        /// <summary>
        /// Returns a bool indicating that the recieved string is good
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool StringIsGood(string s)
        {
            char[] readArr = s.ToArray();

            ushort crc = 0;

            for(int i = 2; i < 98; i += 2)
            {
                crc += (ushort)(readArr[i] + (ushort)(256 * readArr[i + 1]));
            }

            if (readArr[100] != 0xFF    ||
                readArr[101] != 0xFE    ||     // end characters in the correct place
                readArr.Length != BufferSize ||     // length is correct
                readArr[0] > 5    ||           // segment indicator is reporting reasonable values
                crc == readArr[98] + readArr[99] * 256  // checksum is correct
                ) { return false; }

            return true;
        }

        /// <summary>
        /// Returns a bool indicating that the recieved buffer is good
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool BufferIsGood(byte[] buffer)
        {
            ushort crc = 0;

            for (int i = 2; i < 98; i += 2)
            {
                crc += (ushort)(buffer[i] + (ushort)(256 * buffer[i + 1]));
            }

            if (buffer[100] != 0xFF ||
                buffer[101] != 0xFE ||     // end characters in the correct place
                buffer.Length != 104 ||     // length is correct
                buffer[0] > 5 ||           // segment indicator is reporting reasonable values
                crc != buffer[98] + buffer[99] * 256  // checksum is correct
                )
            { return false; }

            return true;
        }

        #endregion

        #region Instance Members

        /// <summary>
        /// Event raised when a frame is recieved
        /// </summary>
        public event frameRecieved OnFrameUpdated;
        public delegate void frameRecieved(DataFrame f, SerialInterface sender);

        /// <summary>
        /// Event raised when the buffer is updated
        /// </summary>
        private event bufferUpdated OnBufferUpdated;
        private delegate void bufferUpdated(byte[] buffer);

        /// <summary>
        /// Serialport object
        /// </summary>
        private SerialPort serial;

        /// <summary>
        /// Returns the open status of this serial connection
        /// </summary>
        /// <returns></returns>
        public bool IsOpen 
        {
            get { return serial.IsOpen; }
        }

        /// <summary>
        /// The name of the port
        /// </summary>
        public string Name
        {
            get { return serial.PortName; }
        }

        /// <summary>
        /// The number of consecutive read failures
        /// </summary>
        public int ConsecutiveReadFails = 0;

        /// <summary>
        /// The last recieved frame from this interface
        /// </summary>
        public DataFrame LastFrame
        {
            get { return lastFrame; }
            private set { lastFrame = value; }
        }
        private DataFrame lastFrame;

        #region Initialisation Connection and Disconnection

        /// <summary>
        /// Constructor creates a serial port on the selected port
        /// </summary>
        /// <param name="portName"></param>
        public SerialInterface(string portName)
        {
            Initialise(portName, 250000);
        }

        /// <summary>
        /// Constructor creates a serial port on the selected port with a specified baudrate
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        public SerialInterface(string portName, int baudRate)
        {
            Initialise(portName, baudRate);
        }

        /// <summary>
        /// Function called on initialisation
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudrate"></param>
        private void Initialise(string portName, int baudrate)
        {
           
            // initialise the serial port
            serial = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One);

            // set the recieved bytes threshold
            serial.ReceivedBytesThreshold = BufferSize;
            serial.Open();

            // discard buffers
            serial.DiscardInBuffer();
            serial.DiscardInBuffer();

            // subscribe to event
            serial.DataReceived += Serial_DataReceived;

            // set endline
            char[] chars = { (char)0xFF, (char)0xFE };
            string newline = new string(chars);
            serial.NewLine = newline;
            

            if (serial.IsOpen)
            {
                if (!Initialised)
                {
                    OpenInterfaces = new Dictionary<string, SerialInterface>();
                    Initialised = true;
                }
                
                OpenInterfaces.Add(this.Name, this);
                OnBufferUpdated += BufferUpdated;
            }
        }

        /// <summary>
        /// Closes the serial port and disposes the SerialPort object
        /// </summary>
        public void Close()
        {
            serial.Close();
            serial.Dispose();
        }

        #endregion

        #region Recieving Parsing and Handling Data

        /// <summary>
        /// Event handler for when the input buffer is updated
        /// </summary>
        /// <param name="buffer"></param>
        private void BufferUpdated(byte[] buffer)
        {
            // check the buffer is good
            if(SerialInterface.BufferIsGood(buffer))
            {
                DataFrame f = new DataFrame(buffer);
                LastFrame = f;
                OnFrameUpdated?.Invoke(f, this);
                ConsecutiveReadFails = 0;
                return;
            }

            #if DEBUG
            Console.WriteLine("Buffer Read failed");
            #endif

            CleanInBuffer();
        }

        /// <summary>
        /// Event handler for serial port's data recieved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // read the buffer
            try
            {
                //string read = serial.ReadLine();
                byte[] buffer = new byte[BufferSize];
                serial.Read(buffer, 0, BufferSize);
                OnBufferUpdated?.Invoke(buffer);
            }
            catch(Exception err)
            {
                Console.WriteLine("ERROR: " + err.Message);
            }

        }

        #endregion

        #region Clearing Buffer

        private Timer WipeInputBufferTimer;
        private int wipeCount = 0;

        /// <summary>
        /// Starts the process of wiping the input buffer when something goes wrong
        /// </summary>
        private void CleanInBuffer()
        {
            WipeInputBufferTimer = new Timer(30);
            
            WipeInputBufferTimer.Elapsed += WipeBuffer;
            WipeInputBufferTimer.Start();
            #if DEBUG
            Console.WriteLine("Clearing Input Buffer");
            #endif

        }

        /// <summary>
        /// Event handler for the timer initialised by CleanInBuffer(), wipes the input buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WipeBuffer(object sender, ElapsedEventArgs e)
        {
            if(wipeCount < 5)
            {
                serial.DiscardInBuffer();
                wipeCount++;
            }
            else
            {
                wipeCount = 0;
                WipeInputBufferTimer.Elapsed -= WipeBuffer;
                WipeInputBufferTimer.Stop();
            }
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// class representing a single frame
    /// </summary>
    public class DataFrame
    {
        #region Static Members

        public static bool IsValid(string readString)
        {
            // check the end characters
            return false;
        }

        #endregion

        #region Instance Members

        // raw string
        private string ReadString;

        /// <summary>
        /// The ID of the segment
        /// </summary>
        public int segmentID;

        /// <summary>
        /// The Voltages in this segment
        /// </summary>
        public float[] Voltages = new float[24];

        /// <summary>
        /// The Temperatures in this segment
        /// </summary>
        public float[] Temperatures = new float[24];

        /// <summary>
        /// Maximum Voltage in this segment
        /// </summary>
        public float maxVoltage
        {
            get { return Voltages.Max(); }
        }

        /// <summary>
        /// Minimum voltage in this segment
        /// </summary>
        public float minVoltage
        {
            get { return Voltages.Min(); }
        }

        /// <summary>
        /// Maximum temperature in this segment
        /// </summary>
        public float maxTemp
        {
            get { return Temperatures.Max(); }
        }

        /// <summary>
        /// Minimum temperature in this segment
        /// </summary>
        public float minTemp
        {
            get { return Temperatures.Min(); }
        }

        /// <summary>
        /// The voltage range within this segment 
        /// </summary>
        public float voltageRange
        {
            get { return (maxVoltage - minVoltage); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="readString"></param>
        public DataFrame(string readString)
        {
            ReadString = readString;

            // convert to char array
            char[] arr = ReadString.ToCharArray();

            // populate data members
            segmentID = arr[0];
            int j = 0;
            for(int i = 2; i < 50; i += 2)
            {
                Voltages[j] = arr[i] + arr[i + 1] * 256;
                Temperatures[j] = arr[i + 48] + arr[i + 49] * 256;
                j++;
            }

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="arr"></param>
        public DataFrame(byte[] arr)
        {
            // populate data members
            segmentID = arr[0];
            int j = 0;
            for (int i = 2; i < 50; i += 2)
            {
                Voltages[j] = arr[i] + arr[i + 1] * 256;
                Voltages[j] /= 100;
                Temperatures[j] = arr[i + 48] + arr[i + 49] * 256;
                Temperatures[j] /= 100;
                j++;
            }
        }

        #endregion
    }
}
