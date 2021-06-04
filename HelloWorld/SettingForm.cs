﻿using System;
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
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;

namespace IOTClient
{
    public partial class SettingForm : Form
    {
        private dynamic client = APIHelper.init();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //On screen attribute
        //private string auto_tint_id = ConfigurationManager.AppSettings.Get("auto_tint_id");
        //private string csv_history_path = ConfigurationManager.AppSettings.Get("csv_history_path");
        //private string database_path = ConfigurationManager.AppSettings.Get("database_path");
        private string auto_tint_id = ManageConfig.ReadConfig("auto_tint_id");
        private string csv_history_path = ManageConfig.ReadConfig("csv_history_path");
        public static string database_path = ManageConfig.ReadConfig("database_path");

        //For export and move file
        private string jsonDispenseLogPath = ConfigurationManager.AppSettings.Get("json_dispense_log_path");
        private string csv_history_achive_path = ConfigurationManager.AppSettings.Get("csv_history_achive_path");

        public SettingForm()
        {

            InitializeComponent();
            
            //this.Load = SettingForm_Load;

        }

        private async void SettingForm_Load(object sender, EventArgs e)
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
            databaseLocationTextbox.Text = database_path;
            posHistoryLocationTextBox.Text = csv_history_path;

            LoadGlobalConfig();
            CheckLastestUploadDateTime();
            await UpdateAutotintVersion();


        }

        private void CheckLastestUploadDateTime()
        {
            string LatestExportDateTime = "";
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            var directory = new DirectoryInfo(path);
            if(directory.GetFiles().Length > 0)
            {
                var latestFileInfo = (from f in directory.GetFiles()
                                      orderby f.LastWriteTime descending
                                      select f).First();
                LatestExportDateTime = latestFileInfo.CreationTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));
            }
            else
            {
                LatestExportDateTime = "ไม่พบประวัติการส่งออกข้อมูล";
            }
           
            HistoryExportDateTime.Text = $"{LatestExportDateTime}";
        }

        private void LoadGlobalConfig()
        {
            tbxShopDispenVal.Text = ManageConfig.ReadGlobalConfig("auto_tint_id");
            posHistoryLocationTextBox.Text = ManageConfig.ReadGlobalConfig("csv_history_path");
            databaseLocationTextbox.Text = ManageConfig.ReadGlobalConfig("database_path");
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {

            await UpdateAutotintVersion();
            
        }

        private async Task UpdateAutotintVersion()
        {
            string str_response = await APIHelper.GetAutoTintVersion(client, auto_tint_id);

            APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(str_response);

            if (response.statusCode == 200)
            {
                AutoTintWithId result = JsonConvert.DeserializeObject<AutoTintWithId>(response.message);
                lblDatabaseVersionText.Text = "" + result.pos_setting_version.id;
                DateTime startTimeFormate = DateTime.UtcNow;
                TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
                string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));

                lblDatabaseCheckVal.Text = ICTDateTimeText;
                //Check the server for newer version.
                PrismaProLatestVersion checkVersion = await APIHelper.GetDBLatestVersion(client, result.pos_setting_version.id);
                Logger.Info($"Successful on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
                if (result.pos_setting_version.id < checkVersion.id)
                {
                    //Goto download
                    MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"รุ่นของฐานข้อมูลไม่ใช่รุ่นล่าสุด \n รุ่นปัจจุบัน : {result.pos_setting_version.id} \n รุ่นล่าสุด : {checkVersion.id}", "แจ้งเตือน", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"Status Code : {response.statusCode} \nMessage : {response.message}", "Error", MessageBoxButton.OK);
                Logger.Error($"Exception on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
            }
        }

        private void SaveInputData_Click(object sender, EventArgs e)
        {
            if (databaseLocationTextbox.Text != "" || posHistoryLocationTextBox.Text != "")
            {
                ManageConfig.WriteGlobalConfig("auto_tint_id", tbxShopDispenVal.Text);
                ManageConfig.WriteGlobalConfig("database_path", databaseLocationTextbox.Text);
                ManageConfig.WriteGlobalConfig("csv_history_path", posHistoryLocationTextBox.Text);
                MessageBoxResult confirmResult = System.Windows.MessageBox.Show("บันทึกค่าเสร็จสิ้น", "สำเร็จ", MessageBoxButton.OK);
            }
            else
            {
                MessageBoxResult confirmResult = System.Windows.MessageBox.Show("กรุณาเติมแบบฟอร์มทุกช่องก่อนกดบันทึก", "ผิดพลาด", MessageBoxButton.OK);
            }

        }

        private void btnDatabaseSelect_Click(object sender, EventArgs e)
        {
            var fsd = new FolderSelectDialog();
            fsd.Title = "กรุณาเลือกที่ตั้งฐานข้อมูล";
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
            fsd.Title = "กรุณาเลือกที่ตั้งประวัติ";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                posHistoryLocationTextBox.Text = fsd.FileName;
            }
        }

        private void btnExport1_Click(object sender, EventArgs e)
        {
            var instance = new FileOperationLibrary();

            Thread progressThread = new Thread(delegate ()
            {
                LoadingForm progress = new LoadingForm();
                progress.ShowDialog();
            });

            progressThread.Start();
            instance.StartOperation();
            var utcTime = DateTime.UtcNow;
            var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
            logMaskAsDoneDate("" + actualTime);

            progressThread.Abort();
            CheckLastestUploadDateTime();
        }

        private void btnCheckShopID_Click(object sender, EventArgs e)
        {
            ManageConfig.ReadGlobalConfig("global_config_path");
        }

        private static void logMaskAsDoneDate(string text)
        {
            Console.WriteLine("Call logger !!");
            Logger.Trace("Done for today " + text);
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
