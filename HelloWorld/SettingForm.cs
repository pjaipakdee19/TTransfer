using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;

using AutoTintLibrary;

namespace IOTClient
{
    public partial class SettingForm : Form
    {
        private dynamic client = APIHelper.init();
        //On screen attribute
        private string auto_tint_id = ConfigurationManager.AppSettings.Get("auto_tint_id");
        private string csv_history_path = ConfigurationManager.AppSettings.Get("csv_history_path");
        private string database_path = ConfigurationManager.AppSettings.Get("database_path");

        //For export and move file
        private string jsonDispenseLogPath = ConfigurationManager.AppSettings.Get("json_dispense_log_path");
        private string csv_history_achive_path = ConfigurationManager.AppSettings.Get("csv_history_achive_path");
        private string folderName;

        public SettingForm()
        {

            InitializeComponent();
            
            this.Load += SettingForm_Load;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblHelloWorld.Text = "Hello World!";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
        //Load configuration from json file or xml

        //MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Are you sure to delete this item ??", "Confirm Delete!!", MessageBoxButton.YesNo);
        //if (confirmResult == MessageBoxResult.Yes)
        //{
        //    // If 'Yes', do something here.
        //}
        //else
        //{
        //    // If 'No', do something here.
        //}
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            string auto_tint_id = "12345678AT01";
            AutoTintWithId result = await APIHelper.GetAutoTintVersion(client,auto_tint_id);
            //Debug.WriteLine(result);
            //AutoTintWithId myDeserializedClass = JsonConvert.DeserializeObject<AutoTintWithId>(result);
            lblDatabaseVersionText.Text = ""+ result.pos_setting_version.id;
            DateTime startTimeFormate = DateTime.UtcNow;
            TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
            lblDatabaseCheckVal.Text = ""+localDateTime;
            //Check the server for newer version.
            PrismaProLatestVersion checkVersion = await APIHelper.GetDBLatestVersion(client, result.pos_setting_version.id);
            if(result.pos_setting_version.id < checkVersion.id)
            {
                //Goto download
            }
        }

        private void SaveInputData_Click(object sender, EventArgs e)
        {
            //MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Are you sure to delete this item ??", "Confirm Delete!!", MessageBoxButton.YesNo);
            //if (confirmResult == MessageBoxResult.Yes)
            //{
            //    // If 'Yes', do something here.
            //}
            //else
            //{
            //    // If 'No', do something here.
            //}
            if (databaseLocationTextbox.Text != "" || posHistoryLocationTextBox.Text != "")
            {
                //database_path = databaseLocationTextbox.Text;
                AddOrUpdateAppSettings("database_path", databaseLocationTextbox.Text);
                AddOrUpdateAppSettings("csv_history_path", posHistoryLocationTextBox.Text);
            }
            string test = ConfigurationManager.AppSettings.Get("database_path");
            MessageBoxResult confirmResult = System.Windows.MessageBox.Show(test,"Dialog Title", MessageBoxButton.OK);
        }

        private void btnDatabaseSelect_Click(object sender, EventArgs e)
        {
            var fsd = new FolderSelectDialog();
            fsd.Title = "What to select";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                databaseLocationTextbox.Text = fsd.FileName;
            }
        }

        private void btnSeletectHistoryCVS_Click(object sender, EventArgs e)
        {
            var fsd = new FolderSelectDialog();
            fsd.Title = "What to select";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                posHistoryLocationTextBox.Text = fsd.FileName;
            }
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        //private void Form1_Resize(object sender, System.EventArgs e)
        //{
        //    if (FormWindowState.Minimized == WindowState)
        //        Hide();
        //}

        //private void SettingForm_Load(object sender, EventArgs e)
        //{

        //}
    }
}
