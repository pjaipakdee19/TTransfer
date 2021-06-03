using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using CsvHelper;
using NLog;
using Newtonsoft.Json;
using CsvHelper.Configuration;
using System.Globalization;

namespace AutoTintLibrary
{
    public class FileOperationLibrary
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };
        public void StartOperation()
        {
            //Init configuration variable
            string csv_history_path = ConfigurationManager.AppSettings.Get("csv_history_path");
            string jsonDispenseLogPath = ConfigurationManager.AppSettings.Get("json_dispense_log_path");
            string csv_history_achive_path = ConfigurationManager.AppSettings.Get("csv_history_achive_path");
            string auto_tint_id = ConfigurationManager.AppSettings.Get("auto_tint_id");
            //find the csv in history files
            DirectoryInfo csvHistoryPathInfo = new DirectoryInfo(csv_history_path);
            foreach (var csvFile in csvHistoryPathInfo.GetFiles("*.csv"))
            {
                var reader = new StreamReader(csvFile.FullName);
                var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<DispenseHistory>().ToList();

                var dateList = new List<string>();
                //does csv file exist?
                //Extract the csv to json following DISPENSED_DATE
                //Get all date in csv
                for (int i = 0; i < records.Count(); i++)
                {
                    string[] date = records[i].dispensed_date.Split(' ');
                    dateList.Add(date[0]);

                }

                string[] cleanDate = RemoveDuplicates(dateList);


                //save the dispenselog to file.json following date
                for (int i = 0; i < cleanDate.Count(); i++)
                {
                    var exportRecord = new List<DispenseHistory>();
                    var exportRecordBI = new List<DispenseHistoryBI>();
                    for (int j = 0; j < records.Count(); j++)
                    {

                        if (records[j].dispensed_date.Contains(cleanDate[i]))
                        {
                            exportRecord.Add(records[j]);
                            //exportRecordBI.Add(convertToBIData(cleanDate[i],auto_tint_id,records[j]));
                        }

                    }
                    if (exportRecord.Count > 0)
                    {
                        var export_path = jsonDispenseLogPath + "\\" + "full_dispense_log_" + cleanDate[i].Replace("/", "_") + ".json";

                        Logger.Info("Export log path : " + export_path);
                        File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord));

                        //var export_path_bi = jsonDispenseLogPath + "\\" + "full_dispense_log_" + cleanDate[i].Replace("/", "_") + "_BI.json";
                        //Logger.Info("Export bi log" + export_path_bi);
                        //File.WriteAllText(export_path_bi, JsonConvert.SerializeObject(exportRecordBI));
                    }
                }

                //Move complete extract file into achive folder
                reader.Close();
                new System.IO.FileInfo(csv_history_achive_path).Directory.Create();
                File.Move(csvFile.FullName, csv_history_achive_path + "\\" + csvFile.Name);
            }


            //N find json files in json dir, Does the files is exist ?
            DirectoryInfo jsonDispensePathInfo = new DirectoryInfo(jsonDispenseLogPath);
            foreach (var jsonFile in jsonDispensePathInfo.GetFiles("*.json"))
            {
                try
                {
                    //Y Did the files name ended with _p2 ? (Does it have file name not end with p2 ?)
                    if (!jsonFile.Name.Contains("_p2"))
                    {

                        int retry = 1;
                        while (retry <= 3)
                        {
                            //send to api

                            //success mv change filename end with _p2
                            //string[] mvFile = Directory.GetFiles(jsonFile.FullName);
                            int extensionIndex = jsonFile.Name.IndexOf(".json");

                            File.Move(jsonFile.FullName, jsonDispenseLogPath + "\\" + jsonFile.Name.Substring(0, extensionIndex) + "_p2.json");
                            Logger.Info("Transfer to server complete move json files to : " + jsonDispenseLogPath + "\\" + jsonFile.Name.Substring(0, extensionIndex) + "_p2.json");
                            retry = 4;
                        }
                        if (retry > 3)
                        {
                            Logger.Error("Exception on send to api");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception on create json _p2 : " + ex.ToString());
                }

            }
        }

        private DispenseHistoryBI convertToBIData(string clean_date, string auto_tint_id, DispenseHistory dispenseHistory)
        {
            DispenseHistoryBI data = new DispenseHistoryBI();
            string recordKeyDate = clean_date.Split('_')[2] + clean_date.Split('_')[1] + ((clean_date.Split('_')[0].Length < 2) ? "0" : "") + clean_date.Split('_')[0];
            data.record_key = "ID" + recordKeyDate + auto_tint_id;
            return data;
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
