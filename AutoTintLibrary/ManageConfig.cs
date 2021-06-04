using AutoTintLibrary.Model.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
    public class ManageConfig
    {
        private static string GlobalConfigPath = @"C:\ProgramData\TOA_Autotint\config.json";
        public static string ReadGlobalConfig(string key)
        {
            GlobalConfig item = new GlobalConfig();
            if (!File.Exists(GlobalConfigPath))
            {
                GlobalConfig conf = new GlobalConfig();
                conf.global_config_path = GlobalConfigPath;
                string JSONresult = JsonConvert.SerializeObject(conf);
                using (var tw = new StreamWriter(GlobalConfigPath, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            using (StreamReader r = new StreamReader(GlobalConfigPath))
            {
                string json = r.ReadToEnd();
                item = JsonConvert.DeserializeObject<GlobalConfig>(json);
            }

            System.Reflection.PropertyInfo pi = item.GetType().GetProperty(key);
            String returnValue = (String)(pi.GetValue(item, null));


            return returnValue;
        }

        public static string WriteGlobalConfig(string key,string value)
        {
            GlobalConfig item = new GlobalConfig();
            if (!File.Exists(GlobalConfigPath))
            {
                GlobalConfig conf = new GlobalConfig();
                conf.global_config_path = GlobalConfigPath;
                conf.auto_tint_id = "";
                string JSONresult = JsonConvert.SerializeObject(conf);
                using (var tw = new StreamWriter(GlobalConfigPath, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            else
            {
                //JObject o1 = JObject.Parse(File.ReadAllText(GlobalConfigPath));
                GlobalConfig OldConfig = JsonConvert.DeserializeObject<GlobalConfig>(File.ReadAllText(GlobalConfigPath));
                //Load old value to item

                item.global_config_path = OldConfig.global_config_path;
                item.auto_tint_id = OldConfig.auto_tint_id;
                item.csv_history_path = OldConfig.csv_history_path;
                item.database_path = OldConfig.database_path;

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
        public static string ReadConfig(string key)
        {
            Console.WriteLine($"Read config called!!! : {key} return value {ConfigurationManager.AppSettings.Get(key)}");
            return ConfigurationManager.AppSettings.Get(key);
        }

        public static string WriteConfig(string key,string value)
        {
            AddOrUpdateAppSettings(key, value);
            return ReadConfig(key);
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
