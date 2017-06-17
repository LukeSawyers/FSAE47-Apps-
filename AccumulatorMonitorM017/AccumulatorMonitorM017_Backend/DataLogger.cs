using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccumulatorMonitorM017.Backend
{
    /// <summary>
    /// Provides all the functionality for logging incoming data
    /// </summary>
    public class DataLogger
    {
        private string Dir;
        private string Path;
        private Dictionary<int, string> saveFiles = new Dictionary<int, string>();
        private string TimeWhenStartedString;

        /// <summary>
        /// Constructor writes to sub-directory "Log Files"
        /// </summary>
        public DataLogger()
        {
            Initialise("\\Log Files");
        }

        /// <summary>
        /// Constructor, writes to a specified sub directory
        /// </summary>
        /// <param name="fileDirectory"></param>
        public DataLogger(string fileDirectory)
        {
            Initialise(fileDirectory);
        }

        /// <summary>
        /// Initialisation operations for this classes, called by each constructor
        /// </summary>
        /// <param name="fileDirectory"></param>
        private void Initialise(string fileDirectory)
        {
            TimeWhenStartedString = ((DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt")).Replace(':', '-').Replace('/', '-'));
            Dir = AppDomain.CurrentDomain.BaseDirectory;
            Path = fileDirectory + " " + TimeWhenStartedString;

            Directory.CreateDirectory(Dir + Path);
        }

        /// <summary>
        /// Logs a new dataframe to the file for that segment
        /// </summary>
        /// <param name="f"></param>
        public bool Log(DataFrame f)
        {
            try
            {
                if (!saveFiles.ContainsKey(f.segmentID))
                {
                    // add to the dictionary
                    saveFiles.Add(f.segmentID, fullpath(f.segmentID));

                    // add the header to the file

                    File.AppendAllLines(fullpath(f.segmentID), RetrieveFileHeader(f.segmentID));
                }

                // initialise the string array
                string[] s = new string[]
                {
                DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + ","
                };


                for (int i = 0; i < f.Voltages.Length; i++)
                {
                    s[0] += f.Voltages[i].ToString("0.00") + "," + f.Temperatures[i].ToString("0.00") + ",";
                }

                // write the data
                System.IO.File.AppendAllLines(fullpath(f.segmentID), s);
                
            }
            catch
            {
                return false;
            }

            return File.Exists(fullpath(f.segmentID));
            
        }

        /// <summary>
        /// Unimplemented
        /// </summary>
        /// <param name="acc"></param>
        public void Log(AccumulatorInterface acc)
        {

        }

        #region Helpers

        /// <summary>
        /// Provides the full path for a given dataframe file
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        private string fullpath(int ind)
        {
            return Dir + Path + "\\Segment " + ind + ".csv";
        }

        /// <summary>
        /// Returns the formatted header for a specified index
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        private string[] RetrieveFileHeader(int ind)
        {
            return new string[] {
                "Segment " + ind + "Accumulator Monitor Log File" + TimeWhenStartedString,
                "Time,Cell 1,,Cell 2,,Cell 3,,Cell 4,,Cell 5,,Cell 6,,Cell 7,,Cell 8,,Cell 9,,Cell 10,,Cell 11,,Cell 12,,Cell 13,,Cell 14,,Cell 15,,Cell 16,,Cell 17,,Cell 18,,Cell 19,,Cell 20,,Cell 21,,Cell 22,,Cell 23,,Cell 24",
                ",V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V,T,V",
            };
        }

        #endregion
    }
}
