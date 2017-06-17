using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccumulatorMonitorM017.Backend;

namespace AccumulatorMonitorM017.console
{
    class MonitorApplication
    {
        public bool active = true;

        private string Cmd = "";

        /// <summary>
        /// Accumulator interface
        /// </summary>
        AccumulatorInterface acc = new AccumulatorInterface();

        bool _allstreams = false;
        bool allstreams
        {
            get
            {
                return _allstreams;
            }
            set
            {
                _allstreams = value;
                for (int i = 0; i < 6; i++) { streambools[i] = _allstreams; }
            }
        }
        bool[] streambools = new bool[] { false, false, false, false, false, false };

        public void Start()
        {
            acc.OnAutoConnectSuccessful += Acc_OnAutoConnectSuccessful;
            Console.WriteLine("Accumulator Monitor Application CLI");
            Console.WriteLine("Written for the University of Auckland Formula-SAE Team (FSAE:47) By Luke Sawyers - 2017");
            Console.WriteLine("Please do report issues to https://github.com/likeasomeboody/FSAE47-Apps- and/or contribute");
            Console.WriteLine("\nType 'Help' To get Started");
        }
        
        public void Update()
        {
            // Process Commands
            Cmd = Console.ReadLine();
            ParseCommand(Cmd);
        }

        private string[] HelpText =
        {
            "\nhelp - How did you get here?\n",
            "echo [text] - Prints your own gibberish for you to read\n",
            "quit - Starts the application (no the opposite)\n",

            "connect \t\t\t - attempt to autoconnect to a plugged in accumulator using the default baudrate of 250000",
            "connect [port] \t\t\t - attempt to the selected port using the default baudrate of 250000",
            "connect [port] [baudrate] \t - attempt to the selected port using the selected baudrate\n",

            "get ports \t\t\t - get the available serial ports",
            "get connected \t\t\t - get the connected serial ports",
            "get voltages [segment] \t\t - get the last reported voltages for the specified segment (1-6)\n",
            
            "stream [segment] \t\t - starts/stops streaming the specified segment (1-6)",
            "stream all \t\t\t - starts/stops streaming all segments",
        };

        /// <summary>
        /// Translates commands into actions
        /// </summary>
        /// <param name="cmd"></param>
        private void ParseCommand(string cmd)
        {
            // split the string
            string[] args = cmd.Split(' ');
            string[] lowers = cmd.ToLower().Split(' ');

            if (lowers.Length < 1) { return; }

            switch (lowers[0])
            {
                #region Quit
                case "quit":
                    active = false;
                    break;
                #endregion

                #region Echo
                case "echo":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Not Enough Args");
                        return;
                    }

                    string outstring = "";

                    for (int i = 1; i < args.Length; i++)
                    {
                        outstring += args[i] + " ";
                    }

                    Console.WriteLine(outstring);
                    break;
                #endregion

                #region Get
                case "get":

                    if (args.Length < 2)
                    {
                        Console.WriteLine("Not Enough Args");
                        return;
                    }

                    switch (lowers[1])
                    {
                        #region Ports
                        case "ports":

                            string[] ports = SerialInterface.GetAvailablePorts();

                            foreach (string port in ports)
                            {
                                Console.WriteLine(port);
                            }
                            break;
                        #endregion

                        #region Connected
                        case "connected":
                            if(SerialInterface.OpenInterfaces.Count == 0)
                            {
                                Console.WriteLine("There are no open interfaces");
                                return;
                            }
                            foreach (KeyValuePair<string,SerialInterface> j in SerialInterface.OpenInterfaces.ToList())
                            {
                                Console.WriteLine(j.Value.Name);
                            }
                            break;
                        #endregion

                        #region Voltages
                        case "voltages":
                            if(args.Length < 3)
                            {
                                Console.WriteLine("Not Enough Args");
                                return;
                            }

                            DataFrame f;
                            int id = 0;
                            if (Int32.TryParse(args[2], out id))
                            {
                                if (!acc.GetLastFrame(id, out f))
                                {
                                    Console.WriteLine("No data on that segment is available");
                                    return;
                                }
                                else
                                {
                                    int i = 0;
                                    foreach (float v in f.Voltages)
                                    {
                                        Console.WriteLine(i.ToString() + ": " + f.Voltages[i].ToString("0.00"));
                                        i++;
                                    }
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region Stream

                case "stream":
                    if(args.Length < 2)
                    {
                        Console.WriteLine("Not Enough Args");
                    }

                    // parse segment number
                    int segment;
                    if(Int32.TryParse(args[1], out segment))
                    {
                        if(segment < 7 && segment > 0)
                        {
                            streambools[segment-1] = !streambools[segment-1];
                        }
                    }
                    else
                    {
                        if(lowers[1] == "all")
                        {
                            allstreams = !allstreams;
                        }
                    }

                    // if parse failed, try all keyword
                    
                    break;

                #endregion

                #region Connect
                case "connect":

                    if (args.Length < 2)
                    {
                        acc.StartSerialAutoConnect();
                        return;
                    }

                    if (args.Length < 3)
                    {
                        InitialiseSerial(args[1], 250000);
                    }
                    else
                    {
                        int baudrate;
                        if (Int32.TryParse(args[2], out baudrate))
                        {
                            InitialiseSerial(args[1], baudrate);
                        }
                    }
                    break;
                #endregion

                #region Help

                case "help":
                    foreach(string s in HelpText)
                    {
                        Console.WriteLine(s);
                    }
                    break;

                #endregion
            }
        }

        /// <summary>
        /// Initialises the serial class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baud"></param>
        private void InitialiseSerial(string name, int baud)
        {
            try
            {
                SerialInterface serial = new SerialInterface(name, baud);
                if (serial.IsOpen)
                {
                    serial.OnFrameUpdated += Serial_OnFrameUpdated;
                    Console.WriteLine("Connected to port: " + serial.Name);
                    
                }
                
            }
            catch(Exception err)
            {
                Console.WriteLine("Could not Connect: " + err);
            }
            
        }

        /// <summary>
        /// Event handler for frame updates
        /// </summary>
        /// <param name="f"></param>
        private void Serial_OnFrameUpdated(DataFrame f, SerialInterface sender)
        {
            PrintStream();
        }

        /// <summary>
        /// Prints according to the stream
        /// </summary>
        private void PrintStream()
        {
            string S = "";
            S += "Voltages:" + Environment.NewLine;
            
            // foreach cell
            for(int i = 0; i < 24; i++)
            {
                string s = "";

                // foreach segment
                for (int j = 0; j < 6; j++)
                {
                    if(acc.LastFrames.ContainsKey(j))
                    {
                        s += "S" + j.ToString() + "C" + i.ToString() + ": " + acc.LastFrames[j].Voltages[i].ToString("0.00") + "V\t";
                    }
                }
                S += s + Environment.NewLine;
            }
            S += "Temperatures:" + Environment.NewLine;

            // foreach cell
            for (int i = 0; i < 24; i++)
            {
                string s = "";

                // foreach segment
                for (int j = 0; j < 6; j++)
                {
                    if (acc.LastFrames.ContainsKey(j))
                    {
                        s += "S" + j.ToString() + "C" + i.ToString() + ": " + acc.LastFrames[j].Temperatures[i].ToString("0.00") + "C\t";
                    }
                }
                S += s + Environment.NewLine;
            }

            Console.Clear();
            Console.Write(S);
        }

        // When autoconnect is successful
        private void Acc_OnAutoConnectSuccessful()
        {
            Console.WriteLine("Connected!");
            acc.OnFrameRecived += Serial_OnFrameUpdated;
        }
    }
}
