using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using RestSharp;
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

using AutoTintLibrary;

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
        public static RestClient APIclient = APIHelper.init();
        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {

            var utcTime = DateTime.UtcNow;
            var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
            bool isTodayDone = false;
            //string programdata_path = ConfigurationManager.AppSettings.Get("programdata_log_path");
            string programdata_path = ManageConfig.ReadConfig("programdata_log_path");
            DirectoryInfo programdata_info = new DirectoryInfo(programdata_path);
            foreach (var txtFile in programdata_info.GetFiles("*.txt"))
            {
                if (txtFile.Name.Contains(actualTime.ToString("yyyy-MM-dd")))
                {
                    isTodayDone = true;
                    //Logger.Info("Today is done do nothing");
                }

            }
            if (!isTodayDone)
            {
                Logger.Info("Start transfer operation when PC turn on !!!");
                var instance = new FileOperationLibrary();
                await instance.StartOperation();
            }
            Logger.Info("Start timer");
            System.Timers.Timer timScheduledTask = new System.Timers.Timer();
            timScheduledTask.Enabled = true;
            timScheduledTask.Interval = 60 * 1000;
            timScheduledTask.Elapsed += new System.Timers.ElapsedEventHandler(timeScheduledTask_Elapsed);

        }

        protected override void OnStop()
        {
            NLog.LogManager.Shutdown();
        }

        async void timeScheduledTask_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var utcTime = DateTime.UtcNow;
            var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
            bool isTodayDone = false;
            //string programdata_path = ConfigurationManager.AppSettings.Get("programdata_log_path");
            string programdata_path = ManageConfig.ReadConfig("programdata_log_path");
            DirectoryInfo programdata_info = new DirectoryInfo(programdata_path);
            foreach (var txtFile in programdata_info.GetFiles("*.txt"))
            {
                if (txtFile.Name.Contains(actualTime.ToString("yyyy-MM-dd")))
                {
                    isTodayDone = true;
                    //Logger.Info("Today is done do nothing");
                }

            }
            if (!isTodayDone)
            {
                
                // Execute some task
                bool isBetweenStartTime = check_service_starttime(actualTime);
                if (isBetweenStartTime && (randomStartTime == (new DateTime())))
                {
                    Logger.Info("Start Time random time at : " + actualTime);
                    //Random time
                    Random r = new Random();
                    string randomThreshold = ManageConfig.ReadGlobalConfig("start_random_minutes_threshold");
                    int genRand = r.Next(0, Int16.Parse(randomThreshold));
                    randomStartTime = actualTime.AddMinutes(genRand);
                    Logger.Info("Start Time will be : " + randomStartTime);
                }
                if ((actualTime >= randomStartTime) && (randomStartTime != (new DateTime())))
                {
                    //Go working extract log and send process
                    try
                    {
                        var instance = new FileOperationLibrary();
                        await instance.StartOperation();
                        logMaskAsDoneDate("" + actualTime);
                        //Start check for update and download if needed
                        await instance.UpdateAutotintVersion();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Exception on " + ex.ToString());
                    }

                    Logger.Info("Operation Done on : " + randomStartTime);
                    randomStartTime = new DateTime(); //Reset
                }
            }
            
        }

        bool check_service_starttime(DateTime actualTime)
        {
            // Here is the code we need to execute periodically
            //random when 07:30
            string startTime = ManageConfig.ReadGlobalConfig("service_operation_start");
            string runningTillTime = ManageConfig.ReadGlobalConfig("service_operation_stop");
            int startH = Int16.Parse(startTime.Split(':')[0]);
            int startM = Int16.Parse(startTime.Split(':')[1]);
            int tillH = Int16.Parse(runningTillTime.Split(':')[0]);
            int tillM = Int16.Parse(runningTillTime.Split(':')[1]);
            var startDatetime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, startH, startM, 00); //!!! MOVE HARD CODE TO CONFIGURATION FILE
            var tillDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, tillH, tillM, 00); //!!! MOVE HARD CODE TO CONFIGURATION FILE

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

        private static void logMaskAsDoneDate(string text)
        {
            Console.WriteLine("Call logger !!");
            Logger.Trace("Service Done for today " + text);
        }
    }
}
