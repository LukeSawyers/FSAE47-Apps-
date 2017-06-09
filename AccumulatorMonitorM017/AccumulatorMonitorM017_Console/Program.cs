using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccumulatorMonitorM017.console
{
    class Program
    {
        static MonitorApplication App;

        static void Main(string[] args)
        {
            App = new MonitorApplication();
            App.Start();
            while (App.active) { App.Update(); }
        }
    }
}
