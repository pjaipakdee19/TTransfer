using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;

namespace ConsoleApp1
{
    public class ConfigHelper
    {
        public string json_dispense_log_path { get; set; }
    }

    public class Config
    {

        public string initialize()
        {
            var installLocation = AppDomain.CurrentDomain.BaseDirectory;
            var config_directory = installLocation + "\\config\\app.config.json";

            string jsonString = File.ReadAllText(config_directory);
            return jsonString;
        }

        public string getConfig(string jsonString,string name)
        {
            dynamic config_str = JsonConvert.DeserializeObject<dynamic>(jsonString);
            string result = config_str[name];
            return result;
           
        }
    }

    public class XMLConfig
    {
        public string getConfig(string key)
        {

            return ConfigurationManager.AppSettings.Get(key);
            //return "";
        }
    }
}
