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


namespace ConsoleAppDotNetFW
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };
        private static string[] RemoveDuplicates(List<string> dateList)
        {
            HashSet<string> set = new HashSet<string>(dateList);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }

        string csv_history_achive_path = ConfigurationManager.AppSettings.Get("csv_history_achive_path");
        static public async Task Main(string[] args)
        {
            //var client = new RestClient("https://jsonplaceholder.typicode.com/todos/1");
            //client.Authenticator = new HttpBasicAuthenticator("username", "password");
            var client = APIHelper.init();

            //var request = new RestRequest(Method.GET);
            //request.AddHeader("Accept", "application/json");
            //request.Parameters.Clear();
            //var result = client.Execute(request).Content;
            var result = await APIHelper.RequestGet(client, "https://jsonplaceholder.typicode.com/todos/1");
            Console.WriteLine(result);
            Console.WriteLine("Done!!!");
            System.Console.ReadKey();
            
        }
    }
}