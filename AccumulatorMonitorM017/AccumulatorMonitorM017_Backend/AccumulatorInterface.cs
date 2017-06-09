using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccumulatorMonitorM017.Backend
{
    public class AccumulatorInterface
    {
        /// <summary>
        /// The serial interface of the accumulator
        /// </summary>
        private SerialInterface serial;

        /// <summary>
        /// The last frames for each segment, keyed by segment number
        /// </summary>
        private Dictionary<int, DataFrame> LastFrames = new Dictionary<int, DataFrame>();

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
        /// Constructor
        /// </summary>
        public AccumulatorInterface()
        {

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
        }
    }
}
