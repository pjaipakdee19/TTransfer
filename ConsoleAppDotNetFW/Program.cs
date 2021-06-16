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

using AutoTintLibrary;
using System.Reflection;

namespace ConsoleAppDotNetFW
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };
        private static string GlobalConfigPath = @"C:\ProgramData\TOA_Autotint\config.json";
        private static string[] RemoveDuplicates(List<string> dateList)
        {
            HashSet<string> set = new HashSet<string>(dateList);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }
        //static public async Task Main(string[] args)
        static public void Main(string[] args)
        {
            if (File.Exists(GlobalConfigPath))
            {
                File.Delete(GlobalConfigPath);
            }
            var txtCustomerId = "12345678TA01";
            var txtDBLocation = @"C:\ProgramData\TOA_Autotint\DB";
            var txtHistoryLocation = @"C:\ProgramData\TOA_Autotint\CSV";
            WriteGlobalConfig("auto_tint_id", txtCustomerId);
            WriteGlobalConfig("database_path", txtDBLocation);
            WriteGlobalConfig("csv_history_path", txtHistoryLocation);
            WriteGlobalConfig("csv_history_achive_path", $"{txtHistoryLocation}\\csv_achieve");
            WriteGlobalConfig("csv_history_achive_path", $"{txtHistoryLocation}\\json_log");
            WriteGlobalConfig("service_operation_start", "07:30");
            WriteGlobalConfig("service_operation_stop", "07:55");
            WriteGlobalConfig("start_random_minutes_threshold", "25");
            WriteGlobalConfig("programdata_log_path", @"C:\ProgramData\TOA_Autotint\Logs");

        }

        public static string WriteGlobalConfig(string key, string value)
        {
            GlobalConfig item = new GlobalConfig();
            if (!File.Exists(GlobalConfigPath))
            {
                GlobalConfig conf = new GlobalConfig();
                conf.global_config_path = GlobalConfigPath;
                conf.auto_tint_id = "";
                string JSONresult = JsonConvert.SerializeObject(conf);
                var ProgramDataFolderPath = @"C:\ProgramData\TOA_Autotint";
                if (!Directory.Exists(ProgramDataFolderPath))
                {
                    Directory.CreateDirectory(ProgramDataFolderPath);
                }
                File.WriteAllText(GlobalConfigPath, JSONresult);

            }
            else
            {
                GlobalConfig OldConfig = JsonConvert.DeserializeObject<GlobalConfig>(File.ReadAllText(GlobalConfigPath));
                //Load old value to item

                //item.global_config_path = OldConfig.global_config_path;
                //item.auto_tint_id = OldConfig.auto_tint_id;
                //item.csv_history_path = OldConfig.csv_history_path;
                //item.csv_history_achive_path = OldConfig.csv_history_achive_path;
                //item.database_path = OldConfig.database_path;
                //item.json_dispense_log_path = OldConfig.json_dispense_log_path;
                //item.programdata_log_path = OldConfig.programdata_log_path;
                item = OldConfig;

            }

            Type configType = item.GetType();
            PropertyInfo pinfo = configType.GetProperty(key);
            pinfo.SetValue(item, value, null);


            string NewConfJSONresult = JsonConvert.SerializeObject(item);
            using (var tw = new StreamWriter(GlobalConfigPath, false))
            {
                tw.WriteLine(NewConfJSONresult.ToString());
                tw.Close();
            }

            PropertyInfo pi = item.GetType().GetProperty(key);
            String returnValue = (String)(pi.GetValue(item, null));


            return returnValue;
        }
    }
}