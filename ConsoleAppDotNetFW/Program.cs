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
using System.Net;
using System.Net.Sockets;

namespace ConsoleAppDotNetFW
{
    class Program
    {
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };
        public static decimal small_loop_retry_round = 3;
        public static async Task Main(string[] args)
        {
            string programdata_path = "C:\\ProgramData\\TOA_Autotint\\Logs";
            Console.WriteLine(small_loop_retry_round);
            File.Create($"{programdata_path}\\tmp\\network_require.tmp").Dispose();
            bool isSmallLoopDone = false;
            decimal infinite_loop_round = 1;
            //Add the network connection verification loop here
            while (File.Exists($"{programdata_path}\\tmp\\network_require.tmp"))
            {
                Console.WriteLine("File Exist ? : "+File.Exists($"{programdata_path}\\tmp\\network_require.tmp"));
                Console.WriteLine($"Start checking {DateTime.Now}");
                if (!isSmallLoopDone)
                {
                    //Check 30 sec , 3 rounds
                    for (int i = 0; i < small_loop_retry_round; i++)
                    {
                        if (APIHelper.APIConnectionCheck(1, 2))
                        {
                            File.Delete($"{programdata_path}\\tmp\\network_require.tmp");
                            i = 99;
                        }
                        else
                        {
                            Console.WriteLine($"Done retry round {i} {DateTime.Now}");
                            Logger.Error($"Network not ready retring round .... {i} of {small_loop_retry_round}");
                        }
                    }
                    isSmallLoopDone = true;
                }
                else
                {
                    if (APIHelper.APIConnectionCheck(1, 5))
                    {
                        Console.WriteLine("Delete file");
                        File.Delete($"{programdata_path}\\tmp\\network_require.tmp");
                    }
                    else
                    {
                        Console.WriteLine($"Done retry infinity round {infinite_loop_round} {DateTime.Now}");
                        Logger.Error($"Network not ready retring round .... {infinite_loop_round} in 5 minutes");
                        infinite_loop_round++;
                    }
                }
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}