using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PropertySetupAction
{
    public partial class FolderSelectForm : Form
    {
        private static string GlobalConfigPath = @"C:\ProgramData\TOA_Autotint\config.json";
        private bool useOldConfig = false;
        public FolderSelectForm()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            CheckOldConfig();
            this.TopMost = true;
        }

        private void CheckOldConfig()
        {
            if (File.Exists(GlobalConfigPath))
            {
                DialogResult dialogResult = MessageBox.Show("พบค่าเก่าอยู่ในระบบ ต้องการใช้ค่าเก่าหรือไม่ ?", "แจ้งเตือน", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    useOldConfig = true;
                    //Load config from json to txtbox.
                    txtCustomerId.Text = ReadGlobalConfig("auto_tint_id");
                    txtDBLocation.Text = ReadGlobalConfig("database_path");
                    txtHistoryLocation.Text = ReadGlobalConfig("csv_history_path");

                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(txtDBLocation.Text) && !String.IsNullOrEmpty(txtHistoryLocation.Text) && !String.IsNullOrEmpty(txtCustomerId.Text))
                valid = VerifyInputInfo(txtDBLocation.Text, txtHistoryLocation.Text, txtCustomerId.Text);

            if (!valid)
            {
                MessageBox.Show("You license information does not appear to be valid. Please try again.", "Invalid info");
            }
            else
            {
                //Check if file exist or not first
                if (File.Exists(GlobalConfigPath))
                {
                    File.Delete(GlobalConfigPath);
                }
                
                WriteGlobalConfig("auto_tint_id", txtCustomerId.Text);
                WriteGlobalConfig("database_path", txtDBLocation.Text);
                WriteGlobalConfig("csv_history_path", txtHistoryLocation.Text);
                WriteGlobalConfig("csv_history_achive_path",$"{txtHistoryLocation.Text}\\csv_achieve");
                WriteGlobalConfig("csv_history_achive_path", $"{txtHistoryLocation.Text}\\json_log");
                WriteGlobalConfig("service_operation_start","07:30");
                WriteGlobalConfig("service_operation_stop", "07:55");
                WriteGlobalConfig("start_random_minutes_threshold", "25");
                WriteGlobalConfig("programdata_log_path", @"C:\ProgramData\TOA_Autotint\Logs");
                
                this.DialogResult = DialogResult.Yes;
            }
        }
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

        public static string WriteGlobalConfig(string key, string value)
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

        private bool VerifyInputInfo(string dbLocation, string historyLocation, string customerId)
        {
            // Connect to License server check customerId and run algorithm to file in dblocation and historylocation
            return true;
        }

        private void btnDatabaseSelect_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            var fsd = new FolderSelectDialog();
            fsd.Title = "What to select";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                txtDBLocation.Text = fsd.FileName;
                this.TopMost = true;
            }
        }

        private void btnCSVSelect_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            var fsd = new FolderSelectDialog();
            fsd.Title = "What to select";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                txtHistoryLocation.Text = fsd.FileName;
                this.TopMost = true;
            }
        }
    }
}
