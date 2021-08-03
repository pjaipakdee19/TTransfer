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
        public static async Task Main(string[] args)
        {
            FileOperationLibrary instance = new FileOperationLibrary();
            var result = await instance.downloadBaseDB();

            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}