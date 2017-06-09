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


        public void Start()
        {
            
        }

        public void Update()
        {
            // Process Commands
            Cmd = Console.ReadLine();
            ParseCommand(Cmd);
        }

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

                #region Connect
                case "connect":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Not Enough Args");
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
            
        }
    }
}
