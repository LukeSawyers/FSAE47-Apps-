using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AccumulatorMonitorM017.Backend
{
    public class AccumulatorInterface
    {
        #region Fields

        /// <summary>
        /// The serial interface of the accumulator
        /// </summary>
        private SerialInterface serial;

        /// <summary>
        /// 
        /// </summary>
        private DataLogger dataLogger;

        /// <summary>
        /// The last frames for each segment, keyed by segment number
        /// </summary>
        public Dictionary<int, DataFrame> LastFrames = new Dictionary<int, DataFrame>();

        /// <summary>
        /// Timer used to attempt autoconnects when we are connecting
        /// </summary>
        private Timer AutoConnectTimer;

        /// <summary>
        /// Indicator of whether the program is connected to an accumulator
        /// </summary>
        public bool connected = false;

        public event SerialInterface.frameRecieved OnFrameRecived;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AccumulatorInterface()
        {
            AutoConnectTimer = new Timer(500);
            AutoConnectTimer.Elapsed += AutoConnectTimer_Elapsed;
            dataLogger = new DataLogger();
        }

        #region Methods

        /// <summary>
        /// Returns the last frame recieved for the specified segment. Segment must be between 0 and 5
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool GetLastFrame(int segment, out DataFrame f)
        {
            if(segment < 0 || segment > 5) { f = null; return false; }
            return LastFrames.TryGetValue(segment, out f);
        }

        /// <summary>
        /// Establishes a serial connection to an accumulator given a port name, uses a default baudrate
        /// </summary>
        /// <param name="name"></param>
        public void ConnectSerial(string name)
        {
            Connect(name, 250000);
        }

        /// <summary>
        /// Establishes a serial connection to an accumulator given a port name and a baudrate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baudrate"></param>
        public void ConnectSerial(string name, int baudrate)
        {
            Connect(name, baudrate);
        }

        /// <summary>
        /// Establishes serial connection and subscribes to the events
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baudrate"></param>
        private void Connect(string name, int baudrate)
        {
            serial = new SerialInterface(name, 250000);
            serial.OnFrameUpdated += Serial_OnFrameUpdated;
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Stores incoming dataframes into the correct location in the dictionary
        /// </summary>
        /// <param name="f"></param>
        /// <param name="sender"></param>
        private void Serial_OnFrameUpdated(DataFrame f, SerialInterface sender)
        {
            // if it already exists, overwrite, if not, add
            if (LastFrames.ContainsKey(f.segmentID))
            {
                LastFrames[f.segmentID] = f;
            }
            else
            {
                LastFrames.Add(f.segmentID, f);
            }

            dataLogger.Log(f);

            this.OnFrameRecived?.Invoke(f,sender);
        }

        #endregion

        #region AutoConnect

        private string[] ports;
        private int index = 0;

        public delegate void AutoConnectSuccessful();
        public event AutoConnectSuccessful OnAutoConnectSuccessful;

        private Timer WaitForFrameTimer;

        private bool attemptingAutoConnect = false;

        /// <summary>
        /// Starts AutoConnect
        /// </summary>
        public void StartSerialAutoConnect()
        { 
            if (!connected)
            {
                attemptingAutoConnect = true;
                AutoConnectTimer.Start();
            }
        }

        /// <summary>
        /// Stops Autoconnect
        /// </summary>
        public void StopSerialAutoConnect()
        {
            attemptingAutoConnect = false;
            AutoConnectTimer.Stop();
        }

        /// <summary>
        /// Attempts to connect to each available port and waits to recieve data 
        /// </summary>
        private void AttemptConnection()
        {
            // try to connect to the next port
            try { Connect(ports[index], 250000); }

            // if unsuccessful, move to next port
            catch
            {
                index++;
                // if were finished
                if(index == ports.Length)
                {
                    // start the timer again
                    AutoConnectTimer.Start();
                }
                else
                {
                    // move on to the next port
                    AttemptConnection();
                }
                // exit
                return;
            }

            // if successful, wait to recieve a frame
            serial.OnFrameUpdated += WaitForFrameRecieved;

            // set up timeout timer
            WaitForFrameTimer = new Timer(3000);
            WaitForFrameTimer.Start();
            WaitForFrameTimer.Elapsed += WaitForFrameTimeout;
        }

        /// <summary>
        /// Completed Waiting for a frame, move onto the next one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaitForFrameTimeout(object sender, ElapsedEventArgs e)
        {
            // unsubscribe events
            serial.OnFrameUpdated -= WaitForFrameRecieved;
            WaitForFrameTimer.Elapsed -= WaitForFrameTimeout;

            // attempt another connection
            AttemptConnection();

        }

        /// <summary>
        /// Waits for a frame to be recieved 
        /// </summary>
        /// <param name="f"></param>
        /// <param name="sender"></param>
        private void WaitForFrameRecieved(DataFrame f, SerialInterface sender)
        {
            // a frame has been recieved, Rejoice! we have connected

            // unsubscribe events
            serial.OnFrameUpdated -= WaitForFrameRecieved;
            WaitForFrameTimer.Elapsed -= WaitForFrameTimeout;

            // stop trying to connect
            StopSerialAutoConnect();

            // raise event
            OnAutoConnectSuccessful?.Invoke();

        }

        /// <summary>
        /// Timer interrupt to attempt to autoconnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoConnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            // turn off the timer
            AutoConnectTimer.Stop();

            if (attemptingAutoConnect)
            {
                ports = SerialInterface.GetAvailablePorts();
                index = 0;

                // if the serial port exists, disconnect and dispose
                if (serial != null)
                {
                    serial.Close();
                }

                AttemptConnection();
            }
        }

        #endregion
    }
}
