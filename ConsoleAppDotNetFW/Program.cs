using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnitsNet;
using System;
using System.Runtime;
using System.Runtime.InteropServices;

using AutoTintLibrary;
using System.Reflection;
using System.Timers;
namespace ConsoleAppDotNetFW
{
    class Program
    {
        private async void timeScheduledTask_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("test");
        }
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };




        //Creating the extern function...  
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        [Flags]
        enum ConnectionStates
        {
            Modem = 0x1,
            LAN = 0x2,
            Proxy = 0x4,
            RasInstalled = 0x10,
            Offline = 0x20,
            Configured = 0x40,
        }
        static int seconds = 0;
        public static void Main(string[] args)
        {
            //Timer timScheduledTask = new Timer();
            //timScheduledTask.Enabled = true;
            //timScheduledTask.Interval = 60 * 1000;
            //timScheduledTask.Elapsed += new ElapsedEventHandler(timeScheduledTask_Elapsed);

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            Console.WriteLine("Done");
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            seconds++;
        }
    }
}