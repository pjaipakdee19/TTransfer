using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
            };

            var reader = new StreamReader("E:\\Tutorial\\DispenseHistory_full.csv");
            var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<DispenseHistory>().ToList();

            var installLocation = AppDomain.CurrentDomain.BaseDirectory;


            var config_directory = installLocation + "\\config\\app.config.json";

            string jsonString = File.ReadAllText(config_directory);
            var config_str = JsonConvert.DeserializeObject<Config>(jsonString);
            Console.WriteLine(config_str.json_dispense_log_path);

            Console.WriteLine(records[0].DISPENSED_DATE);

            var dateList = new List<string>();

            //Extract the csv to json following DISPENSED_DATE

            //Get all date in csv
            for (int i = 0; i < records.Count(); i++)
            {
                string[] date = records[i].DISPENSED_DATE.Split(' ');
                dateList.Add(date[0]);

            }

            string[] cleanDate = RemoveDuplicates(dateList);


            //save the dispenselog to file.json following date
            for (int i = 0; i < cleanDate.Count(); i++)
            {
                var exportRecord = new List<DispenseHistory>();
                for (int j = 0; j < records.Count(); j++)
                {

                    if (records[j].DISPENSED_DATE.Contains(cleanDate[i]))
                    {
                        exportRecord.Add(records[j]);
                    }

                }
                if (exportRecord.Count > 0)
                {
                    var export_path = installLocation + "\\export_json\\dispenselog_" + cleanDate[i].Replace("/", "_") + ".json";
                    Console.WriteLine(export_path);
                    File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord));
                }
            }
        }

        private static string[] RemoveDuplicates(List<string> dateList)
        {
            HashSet<string> set = new HashSet<string>(dateList);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }
    }
}
