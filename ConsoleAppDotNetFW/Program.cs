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
        static public async Task Main(string[] args)
        {
            string csv_history_path = ConfigurationManager.AppSettings.Get("csv_history_path");
            string jsonDispenseLogPath = ConfigurationManager.AppSettings.Get("json_dispense_log_path");
            string csv_history_achive_path = ConfigurationManager.AppSettings.Get("csv_history_achive_path");

            var client = APIHelper.init();
            //var result = await APIHelper.RequestGet(client, "http://49.229.21.8/poc/company/");
            //Console.WriteLine(result);
            //Console.WriteLine("Done!!!");
            //System.Console.ReadKey();

            var sendfileResult = await APIHelper.UploadFile(client, "http://49.229.21.8/poc/dispense_history/", "E:\\Tutorial\\json_dispense_log\\full_dispense_log_1_2_2016.json");
            //var streamFile = File.ReadAllText("E:\\Tutorial\\json_dispense_log\\full_dispense_log_1_2_2016.json");
            //Console.WriteLine(streamFile);
            Console.WriteLine(sendfileResult);
            Console.WriteLine("Done!!!");
            System.Console.ReadKey();



            ///////Test convert file csv
            //find the csv in history files
            //DirectoryInfo csvHistoryPathInfo = new DirectoryInfo(csv_history_path);
            //foreach (var csvFile in csvHistoryPathInfo.GetFiles("*.csv"))
            //{
            //    var reader = new StreamReader(csvFile.FullName);
            //    var csv = new CsvReader(reader, csvConfig);
            //    var records = csv.GetRecords<DispenseHistory>().ToList();

            //    var dateList = new List<string>();
            //    //does csv file exist?
            //    //Extract the csv to json following DISPENSED_DATE
            //    //Get all date in csv
            //    for (int i = 0; i < records.Count(); i++)
            //    {
            //        string[] date = records[i].dispensed_date.Split(' ');
            //        dateList.Add(date[0]);

            //    }

            //    string[] cleanDate = RemoveDuplicates(dateList);


            //    //save the dispenselog to file.json following date
            //    for (int i = 0; i < cleanDate.Count(); i++)
            //    {
            //        var exportRecord = new List<DispenseHistory>();
            //        for (int j = 0; j < records.Count(); j++)
            //        {

            //            if (records[j].dispensed_date.Contains(cleanDate[i]))
            //            {
            //                exportRecord.Add(records[j]);
            //            }

            //        }
            //        if (exportRecord.Count > 0)
            //        {
            //            var export_path = jsonDispenseLogPath + "\\" + "full_dispense_log_" + cleanDate[i].Replace("/", "_") + ".json";
            //            Logger.Info("Export " + export_path);
            //            File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord));
            //        }
            //    }

            //    //Move complete extract file into achive folder
            //    reader.Close();
            //    new System.IO.FileInfo(csv_history_achive_path).Directory.Create();
            //    File.Move(csvFile.FullName, csv_history_achive_path + "\\" + csvFile.Name);
            //}


        }
    }
}