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
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TAService
{
    public partial class Service1 : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };

        public DateTime randomStartTime = new DateTime();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            
            System.Timers.Timer timScheduledTask = new System.Timers.Timer();
            timScheduledTask.Enabled = true;
            timScheduledTask.Interval = 60 * 1000;
            timScheduledTask.Elapsed += new System.Timers.ElapsedEventHandler(timScheduledTask_Elapsed);
            //try
            //{
            //    Logger.Info("Hello world");
            //    Logger.Warn("Test Warn");
            //    System.Console.ReadKey();
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex, "Goodbye cruel world");
            //}

        }

        protected override void OnStop()
        {
            NLog.LogManager.Shutdown();
        }

        void timScheduledTask_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var utcTime = DateTime.UtcNow;
            var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
            // Execute some task
            bool isBetweenStartTime = check_service_starttime(actualTime);
            if(isBetweenStartTime && (randomStartTime == (new DateTime())))
            {
                //Random time
                Random r = new Random();
                int genRand = r.Next(0, 25);
                randomStartTime = randomStartTime.AddMinutes(genRand);
                Logger.Info("Start Time will be : " + randomStartTime);
            }
            if ((actualTime >= randomStartTime) && (randomStartTime != (new DateTime())))
            {
                //Go working extract log and send process
                try
                {
                    startOperation();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception on "+ex.ToString());
                }


                randomStartTime = new DateTime(); //Reset
            }
        }

        bool check_service_starttime(DateTime actualTime)
        {
            // Here is the code we need to execute periodically
            //random when 07:30
            var startDatetime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 07, 30, 00); //!!! MOVE HARD CODE TO CONFIGURATION FILE
            var tillDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 08, 00, 00); //!!! MOVE HARD CODE TO CONFIGURATION FILE

            //if local timm diff between server time ? > 1 
            //ExecuteCmd exe = new ExecuteCmd();
            //exe.ExecuteCommandSync("net stop w32time");
            //exe.ExecuteCommandSync("net stop w32time");
            //exe.ExecuteCommandSync("net stop w32time");
            //exe.ExecuteCommandSync("net stop w32time");
            //exe.ExecuteCommandSync("net stop w32time");
            //Reboot the PC after execute command !!!???!!!

            return (actualTime > startDatetime && actualTime < tillDateTime);
        }

        void startOperation()
        {
            //Init configuration variable
            string csv_history_path = ConfigurationManager.AppSettings.Get("csv_history_path");
            string jsonDispenseLogPath = ConfigurationManager.AppSettings.Get("json_dispense_log_path");
            string csv_history_achive_path = ConfigurationManager.AppSettings.Get("csv_history_achive_path");
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
                        var export_path = jsonDispenseLogPath + "\\" + "full_dispense_log_" + cleanDate[i].Replace("/", "_") + ".json";
                        Logger.Info("Export " + export_path);
                        File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord));
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

                            File.Move(jsonFile.FullName, jsonDispenseLogPath + "\\" + jsonFile.Name.Substring(0, extensionIndex - 1) + "_p2.json");
                        }
                        if (retry >3)
                        {
                            Logger.Error("Exception on send to api");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception on create json _p2");
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
