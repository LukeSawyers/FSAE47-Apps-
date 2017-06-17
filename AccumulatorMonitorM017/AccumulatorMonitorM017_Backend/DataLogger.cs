using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccumulatorMonitorM017.Backend
{
    /// <summary>
    /// Provides all the functionality for logging incoming data
    /// </summary>
    class DataLogger
    {

        string Dir;
        string Path;
        string Logfile;
        string fullpath { get { return Dir + Path + Logfile; } }

        public DataLogger()
        {
            Initialise("\\Log Files");
        }

        public DataLogger(string fileDirectory)
        {
            Initialise(fileDirectory);
        }

        private void Initialise(string fileDirectory)
        {
            Dir = AppDomain.CurrentDomain.BaseDirectory;
            Path = fileDirectory;
            Logfile = "\\LogFile" + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + ".csv";
            Logfile.Replace(':', '-');

            // add the header
            string[] s = {
                "Accumulator Monitor Log File" + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt"),
                "",
                "",
                "",
            };

            for(int i = 0; i < 288; i++)
            {
                // every even cell
                if ((i % 2) == 0) // i = 0, 2, 4 etc
                {
                    // Unit labels
                    s[3] += "V";

                    // Cell Number
                    s[2] += "Cell " + (i % 24) + 1;
                    
                    // segment label header
                    if ((i % 48) == 0) // i = 0, 48, 96 etc
                    {
                        s[1] += "Segment " + ((i / 48) + 1);
                    }
                }
                // every odd cell
                else
                {
                    // Unit labels
                    s[3] += "T";
                }
                
                // append commas 
                s[1] += ",";
                s[2] += ",";
                s[3] += ",";

            }

            System.IO.File.AppendAllLines(fullpath, s);
        }

        /// <summary>
        /// Logs a new dataframe to the file
        /// </summary>
        /// <param name="f"></param>
        public void Log(DataFrame f)
        {
            string[] s = { "" };
            int startInd = f.segmentID * 24;
            //System.IO.File.AppendAllLines
        }

        public void Log(AccumulatorInterface acc)
        {

        }
    }
}
