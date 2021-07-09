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
using System.Net;
using System.Reflection;

namespace IOTClient
{
    public partial class SettingForm : Form
    {
        private dynamic client = APIHelper.init();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //On screen attribute
        private string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
        private string csv_history_path = ManageConfig.ReadGlobalConfig("csv_history_path");
        private string database_path = ManageConfig.ReadGlobalConfig("database_path");

        dynamic Jsonettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public SettingForm()
        {

            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = Text + " " + version.Major + "." + version.Minor + " (build " + version.Build + ")"; //change form title
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
            if (directory.GetFiles().Length > 0)
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
            tbxShopDispenVal.Text = auto_tint_id;
            posHistoryLocationTextBox.Text = csv_history_path;
            databaseLocationTextbox.Text = database_path;
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            database_path = ManageConfig.ReadGlobalConfig("database_path");
            //await UpdateAutotintVersion();

            //Below is for testing the service.
            //var instance = new FileOperationLibrary();
            //await instance.UpdateAutotintVersion();

        }

        private async Task UpdateAutotintVersion()
        {
            string str_response = await APIHelper.GetAutoTintVersion(client, auto_tint_id);

            APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(str_response);

            if (response.statusCode == 200)
            {
                AutoTintWithId result = JsonConvert.DeserializeObject<AutoTintWithId>(response.message, Jsonettings);
                DateTime startTimeFormate = DateTime.UtcNow;
                TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
                string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));

                lblDatabaseCheckVal.Text = ICTDateTimeText;
                PrismaProLatestVersion checkVersion = new PrismaProLatestVersion();
                //Check the server for newer version.
                //if (result.pos_setting?.id == null)
                //{
                //    //Get the latest version of db

                //    dynamic prima_pro_version_response = await APIHelper.RequestGet(client, "/prisma_pro/");
                //    var lastestJson = JObject.Parse(prima_pro_version_response)["message"];
                //    PrismaPro prisma_pro_attr = JsonConvert.DeserializeObject<PrismaPro>(lastestJson.ToString());
                //    checkVersion = await APIHelper.GetDBLatestVersion(client, prisma_pro_attr.results[0].id);

                //}
                //else
                //{
                checkVersion = await APIHelper.GetDBLatestVersion(client, result.pos_setting.id);
                //}


                Logger.Info($"Successful on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
                //Set version label
                lblDatabaseVersionText.Text = (result.pos_setting_version == null) ? "ไม่พบข้อมูล": $"{result.pos_setting_version.id}";
                var shouldDownloadNewDB = (result.pos_setting_version == null) ? true : (result.pos_setting_version.id < checkVersion.id);
                //if (shouldDownloadNewDB)
                if (true)
                {
                    //    //Goto download
                    MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"รุ่นของฐานข้อมูลไม่ใช่รุ่นล่าสุด \n รุ่นปัจจุบัน : {result.pos_setting_version?.id} \n รุ่นล่าสุด : {checkVersion.id} \n ระบบจะทำการ Download อัตโนมัติ", "แจ้งเตือน", MessageBoxButton.OK);

                   
                    string downloadURI = $"{checkVersion.file}";

                    String[] URIArray = downloadURI.Split('/');
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri(downloadURI), $"{database_path}\\{URIArray[URIArray.Length-1]}");
                    //Update to API about new version of database
                    string data = @"
                    {
                    ""pos_setting_version_id"": " + checkVersion.id + @"
                    }
                    ";
                    dynamic prima_pro_version_response = await APIHelper.RequestPut(client, $"/auto_tint/{auto_tint_id}/pos_update", data);
                    //Update version after complete
                    lblDatabaseVersionText.Text = $"{checkVersion.id}";
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
                //ManageConfig.WriteGlobalConfig("auto_tint_id", tbxShopDispenVal.Text);
                //ManageConfig.WriteGlobalConfig("database_path", databaseLocationTextbox.Text);
                //ManageConfig.WriteGlobalConfig("csv_history_path", posHistoryLocationTextBox.Text);

                ManageConfig.WriteGlobalConfig("auto_tint_id", tbxShopDispenVal.Text);
                ManageConfig.WriteGlobalConfig("database_path", databaseLocationTextbox.Text);
                ManageConfig.WriteGlobalConfig("csv_history_path", posHistoryLocationTextBox.Text);
                ManageConfig.WriteGlobalConfig("csv_history_achive_path", $"{posHistoryLocationTextBox.Text}\\csv_achieve");
                ManageConfig.WriteGlobalConfig("json_dispense_log_path", $"{posHistoryLocationTextBox.Text}\\json_log");
                ManageConfig.WriteGlobalConfig("service_operation_start", "07:30");
                ManageConfig.WriteGlobalConfig("service_operation_stop", "07:55");
                ManageConfig.WriteGlobalConfig("start_random_minutes_threshold", "25");
                ManageConfig.WriteGlobalConfig("programdata_log_path", @"C:\ProgramData\TOA_Autotint\Logs");
                ManageConfig.WriteGlobalConfig("global_config_path", @"C:\ProgramData\TOA_Autotint\config.json");
                string base_url_tmp = ManageConfig.ReadConfig("base_url");
                if(base_url_tmp == null)
                {
                    ManageConfig.WriteGlobalConfig("base_url", "http://49.229.21.7");
                }
                else
                {
                    ManageConfig.WriteGlobalConfig("base_url", base_url_tmp);
                }

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

        private async void btnExport1_ClickAsync(object sender, EventArgs e)
        {
            var instance = new FileOperationLibrary();

            Thread progressThread = new Thread(delegate ()
            {
                LoadingForm progress = new LoadingForm();
                progress.ShowDialog();
            });

            progressThread.Start();
            await instance.StartOperation();
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
            Logger.Info("Manual Transfer done for today " + text);
        }

        private void btnDownloadUpdate_Click(object sender, EventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri("http://49.229.21.7/files/settings/Tint_On_Shop_TuwDkNh.SDF"), @"E:\Tutorial\db_location\Tint_On_Shop_TuwDkNh.SDF");
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            System.Windows.MessageBox.Show("Download completed!");
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
