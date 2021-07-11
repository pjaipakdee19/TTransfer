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
            string manual_path = $"{path}\\Manual";
            FileInfo latestFileInfo = null;
            FileInfo latestManualFileInfo = null;

            var directory = new DirectoryInfo(path);
            if (directory.GetFiles().Length > 0)
            {
                latestFileInfo = (from f in directory.GetFiles()
                                      orderby f.LastWriteTime descending
                                      select f).First();
                //LatestExportDateTime = latestFileInfo.CreationTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));
            }
            var directoryManual = new DirectoryInfo(manual_path);
            if (directoryManual.GetFiles().Length > 0)
            {
                latestManualFileInfo = (from f in directoryManual.GetFiles()
                                  orderby f.LastWriteTime descending
                                  select f).First();
                //LatestExportDateTime = latestManualFileInfo.LastWriteTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));
                //LastWriteTime
            }
            if (latestFileInfo == null && latestManualFileInfo == null) LatestExportDateTime = "Didn't find the transfer history";
            if (latestFileInfo != null && latestManualFileInfo != null)
            {
                int result = DateTime.Compare(latestFileInfo.CreationTime, latestManualFileInfo.LastWriteTime);
                DateTime tmp = (result <= 0) ? latestManualFileInfo.LastWriteTime : latestFileInfo.CreationTime;
                LatestExportDateTime = tmp.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("en-GB"));
            }
            if(LatestExportDateTime == "" && latestFileInfo != null)
            {
                LatestExportDateTime = latestFileInfo.CreationTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("en-GB"));
            }
            if (LatestExportDateTime == "" && latestManualFileInfo != null)
            {
                LatestExportDateTime = latestManualFileInfo.LastWriteTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("en-GB"));
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
            await UpdateAutotintVersion();

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
                //string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));
                string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("en-GB"));
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
                
                var shouldDownloadNewDB = (result.pos_setting_version == null) ? true : (result.pos_setting_version.id < checkVersion.id);
                if (shouldDownloadNewDB)
                //if (true)
                {
                    //    //Goto download
                    MessageBoxResult msgDownloadbox = System.Windows.MessageBox.Show($"The Database is not the latest version \n Current : {result.pos_setting_version?.id} \n Lastest : {checkVersion.id} \n System will continue Download update automatically", "", MessageBoxButton.OK);

                   
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
                }
                //Set version label
                lblDatabaseVersionText.Text = (result.pos_setting_version == null) ? "No data" : $"{checkVersion.number}";
                if (!shouldDownloadNewDB)
                {
                    MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"Database is up to date", "Message", MessageBoxButton.OK);
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

                MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Configuration saved", "Success", MessageBoxButton.OK);
            }
            else
            {
                MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Please fill all the form before save", "Error", MessageBoxButton.OK);
            }

        }

        private void btnDatabaseSelect_Click(object sender, EventArgs e)
        {
            var fsd = new FolderSelectDialog();
            fsd.Title = "Please select the POS database path";
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
            fsd.Title = "Please select the POS history path (CSV file path)";
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
            MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"Manual Upload Finish", "Message", MessageBoxButton.OK);
        }

        private void btnCheckShopID_Click(object sender, EventArgs e)
        {
            ManageConfig.ReadGlobalConfig("global_config_path");
        }

        private static void logMaskAsDoneDate(string text)
        {
            //Console.WriteLine("Call logger !!");
            Logger.Info("Manual Transfer done on " + text);
            Logger.Trace("Manual Transfer done on " + text);
        }

        private void btnDownloadUpdate_Click(object sender, EventArgs e)
        {
            //WebClient webClient = new WebClient();
            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            //webClient.DownloadFileAsync(new Uri("http://49.229.21.7/files/settings/Tint_On_Shop_TuwDkNh.SDF"), @"E:\Tutorial\db_location\Tint_On_Shop_TuwDkNh.SDF");
            string path = @"E:\Tutorial\csv_history\json_log\full_dispense_log_11_10_15.json";
            var instance = new FileOperationLibrary();
            instance.convertToBIDataNew(path);
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            progressBar1.Visible = false;
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
