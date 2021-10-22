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
using IOTClient.Properties;

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
        public delegate void UpdateTransferBtn();
        public delegate void UpdateDownloadDBBtn();
        public delegate void UpdateProgressLbl();
        public delegate void LastestTransferLbl();
        public delegate void LastestDBinfoLbl();
        public bool waitingUpdateAutotintVersion = false;
        bool minimizedToTray;
        NotifyIcon notifyIcon;
        dynamic Jsonettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public SettingForm()
        {

            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = Text + " " + version.Major + "." + version.Minor;// + " (build " + version.Build + ")"; //change form title
            //Text = $"{Text} Beta 2";
            //this.Load = SettingForm_Load;
            //MinimizeToTray();
            notifyIcon = new NotifyIcon();
            notifyIcon.DoubleClick += new EventHandler(NotifyIconClick);
            notifyIcon.Icon = Resources.SystemTrayApp;
            notifyIcon.Text = ProgramInfo.AssemblyTitle;
            notifyIcon.Visible = true;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            var transferButtonThread = new Thread(new ThreadStart(checkTransferButton));
            transferButtonThread.Start();
            var checkDBbuttonThread = new Thread(new ThreadStart(checkDBButton));
            checkDBbuttonThread.Start();
            var serviceStatusLabelThread = new Thread(new ThreadStart(checkServiceLable));
            serviceStatusLabelThread.Start();
            var lastestUploadLabelThread = new Thread(new ThreadStart(checkUploadDTLable));
            lastestUploadLabelThread.Start();
            var checkDBThread = new Thread(new ThreadStart(checkDBservicecomleteFlag));
            checkDBThread.Start();
            //string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            //string pathToMonitor = $@"{path}\tmp\";
            //FileSystemWatcher watcher = new FileSystemWatcher(pathToMonitor);
            //watcher.Changed += new FileSystemEventHandler(OnCheckServiceLableChanged);
            //watcher.EnableRaisingEvents = true;
            //System.Timers.Timer timScheduledTask = new System.Timers.Timer();
            //timScheduledTask.Enabled = true;
            //timScheduledTask.Interval = 1000;
            //timScheduledTask.Elapsed += new System.Timers.ElapsedEventHandler(checkServiceLable);
        }
        #region Thread_of_exportbtn_handler
        public void disblebtnExportHandler()
        {
            btnExport1.Enabled = false;
            btnExport1.Text = "Transfering";
        }
        public void enablebtnExportHandler()
        {
            btnExport1.Enabled = true;
            btnExport1.Text = "Upload POS history";
        }
        public async void checkTransferButton()
        {

            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            while (true)
            {
                try
                {
                    var delayTask = Task.Delay(1000);
                    if (File.Exists($"{path}\\tmp\\running.tmp"))
                    {
                        btnExport1.Invoke(new UpdateTransferBtn(disblebtnExportHandler));
                    }
                    else
                    {
                        btnExport1.Invoke(new UpdateTransferBtn(enablebtnExportHandler));
                    }
                    await delayTask;
                }
                catch (Exception ex)
                {
                    //Logger.Error($"Exception on checkTransferButton : Exception {ex.Message}");
                }
            }
        }
        #endregion
        #region Thread_of_update_db_btn_handler
        public void disblebtnDBHandler()
        {
            button1.Enabled = false;
            button1.Text = "In progress ...";
        }
        public void enablebtnDBHandler()
        {
            button1.Enabled = true;
            button1.Text = "Check for Update";
        }
        public async void checkDBButton()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            while (true)
            {
                try
                {
                    var delayTask = Task.Delay(1000);
                    if (File.Exists($"{path}\\tmp\\dbupdate_running.tmp"))
                    {

                        button1.Invoke(new UpdateDownloadDBBtn(disblebtnDBHandler));
                    }
                    else
                    {
                        button1.Invoke(new UpdateDownloadDBBtn(enablebtnDBHandler));

                    }
                    await delayTask;
                }
                catch (Exception ex)
                {
                    //Logger.Error($"Exception on checkTransferButton : Exception {ex.Message}");
                }
            }
        }
        #endregion
        #region Thread_of_service_label_handler
        public void updateLabelHandler()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            //var pp =  $"{path}\\tmp\\lib_running_log.json";
            if (File.Exists($"{path}\\tmp\\lib_running_log.json")) { 
                string jsonData = File.ReadAllText($"{path}\\tmp\\lib_running_log.json");
                ProgressCounter pg = JsonConvert.DeserializeObject<ProgressCounter>(jsonData);
                ServiceStatusLbl.Text = $"{pg.status} {pg.complete_counter}%";
                //MethodInvoker mi = delegate () { ServiceStatusLbl.Text = $"{pg.status} {pg.complete_counter}%"; };
                //this.Invoke(mi);
            }
            else
            {
                ServiceStatusLbl.Text = "Collecting data";
                //MethodInvoker mi = delegate () { ServiceStatusLbl.Text = "Stand by"; };
                //this.Invoke(mi);
            }
        }
        public void updateLabelHandlerV2()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            //var pp =  $"{path}\\tmp\\lib_running_log.json";
            if (File.Exists($"{path}\\tmp\\lib_running_log.json"))
            {
                string jsonData = File.ReadAllText($"{path}\\tmp\\lib_running_log.json");
                ProgressCounter pg = JsonConvert.DeserializeObject<ProgressCounter>(jsonData);
                //ServiceStatusLbl.Text = $"{pg.status} {pg.complete_counter}%";
                //MethodInvoker mi = delegate () { ServiceStatusLbl.Text = $"{pg.status} {pg.complete_counter}%"; };
                //this.Invoke(mi);
                ServiceStatusLbl.Invoke((MethodInvoker)(() =>
                {
                    ServiceStatusLbl.Text = $"{pg.status} {pg.complete_counter}%";
                }));
            }
            else
            {
                //ServiceStatusLbl.Text = "Collecting data";
                ServiceStatusLbl.Invoke((MethodInvoker)(() =>
                {
                    ServiceStatusLbl.Text = "Collecting data";
                }));
                //MethodInvoker mi = delegate () { ServiceStatusLbl.Text = "Stand by"; };
                //this.Invoke(mi);
            }
        }
        public void clearLabelHandler()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            if ((File.Exists($"{path}\\tmp\\service_not_start.tmp")))
            {
                ServiceStatusLbl.Text = "Timer service not running";
            }
            else
            {
                ServiceStatusLbl.Text = "Stand by";
            }
        }
        public void noInternetLabelHandler()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            if ((File.Exists($"{path}\\tmp\\network_require.tmp")))
            {
                ServiceStatusLbl.Text = "Trying to connect to network";
                button1.Enabled = false;
                btnExport1.Enabled = false;
            }
            else
            {
                ServiceStatusLbl.Text = "Stand by";
                button1.Enabled = true;
                btnExport1.Enabled = true;
            }
        }
        public async void checkServiceLable()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            while (true)
            {
                try
                {
                    var delayTask = Task.Delay(1000);
                    //if (File.Exists($"{path}\\tmp\\lib_running_log.json")) 
                    if ((File.Exists($"{path}\\tmp\\dbupdate_running.tmp")) || File.Exists($"{path}\\tmp\\running.tmp"))
                    {
                        //button1.Invoke(new UpdateDownloadDBBtn(disblebtnDBHandler));
                        ServiceStatusLbl.Invoke(new UpdateProgressLbl(updateLabelHandler));
                        //updateLabelHandler();
                        //MethodInvoker mi1 = new MethodInvoker(updateLabelHandler);
                        //mi1.BeginInvoke();
                    }
                    else if (File.Exists($"{path}\\tmp\\network_require.tmp"))
                    {
                        ServiceStatusLbl.Invoke(new UpdateProgressLbl(noInternetLabelHandler));
                    }
                    else
                    {
                        ServiceStatusLbl.Invoke(new UpdateProgressLbl(clearLabelHandler));
                        //clearLabelHandler();

                    }
                    await delayTask;
                }catch(Exception ex)
                {
                    //Logger.Error($"Exception on checkServiceLable : Exception {ex.Message}");
                }
                
            }
        }

        public void OnCheckServiceLableChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.  
            Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
            if (e.Name == "lib_running_log.json" && e.ChangeType == WatcherChangeTypes.Changed)
            {
                updateLabelHandlerV2();
            }else if(e.Name == "lib_running_log.json" && e.ChangeType == WatcherChangeTypes.Deleted)
            {
                updateLabelHandlerV2();
            }
        }

        #endregion
        #region Thread_of_lastest_upload_datetime
        public async void checkUploadDTLable()
        {
            while (true)
            {
                try
                {
                    var delayTask = Task.Delay(1000);
                    HistoryExportDateTimeLbl.Invoke(new LastestTransferLbl(CheckLastestUploadDateTime));
                    await delayTask;
                }
                catch (Exception ex)
                {
                    //Logger.Error($"Exception on checkTransferButton : Exception {ex.Message}");
                }
            }
        }
        #endregion
        public async void checkDBservicecomleteFlag()
        {
            while (true)
            {
                try
                {
                    var delayTask = Task.Delay(1000);
                    lblDatabaseVersionText.Invoke(new LastestDBinfoLbl(IsCheckDBversionFlagFileExist));
                    await delayTask;
                }
                catch (Exception ex)
                {
                    //Logger.Error($"Exception on checkTransferButton : Exception {ex.Message}");
                }
            }
        }
        public async void IsCheckDBversionFlagFileExist()
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            
            if ((!File.Exists($"{path}\\tmp\\dbupdate_running.tmp")) && (File.Exists($"{path}\\tmp\\dbupdate_version_check.tmp")) && (!File.Exists($"{path}\\tmp\\dbupdate_client_checked.tmp")))
            {
                UpdateAutotintVersion();
                //lblDatabaseVersionText.Text = "Check";
                //lblDatabaseCheckVal.Text = "DMMMM";
            }
            //if((!File.Exists($"{path}\\tmp\\dbupdate_running.tmp")) && (File.Exists($"{path}\\tmp\\dbupdate_version_check.tmp")) && (File.Exists($"{path}\\tmp\\dbupdate_client_checked.tmp")))
            //{
            //    if((lblDatabaseVersionText.Text == "No Information") && (lblDatabaseCheckVal.Text == "No Information"))
            //    {
            //        UpdateAutotintVersion();
            //    }
            //}
            Console.WriteLine($"waitingUpdateAutotintVersion flage value {waitingUpdateAutotintVersion}");
            if (waitingUpdateAutotintVersion)
            {
                UpdateAutotintVersion();
            }
        }
        private async Task checkversion()
        {
        }
        private async void SettingForm_Load(object sender, EventArgs e)
        {
            databaseLocationTextbox.Text = database_path;
            posHistoryLocationTextBox.Text = csv_history_path;

            try
            {
                LoadGlobalConfig();
                //CheckLastestUploadDateTime();
                //Thread nt = new Thread(UpdateAutotintVersion);
                //nt.Start();
                UpdateAutotintVersion();
            }
            catch(Exception ex)
            {
                MessageBoxResult exInitMsgbox = System.Windows.MessageBox.Show($"{ex.Message}", "", MessageBoxButton.OK);
            }
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
                int result = DateTime.Compare(latestFileInfo.LastWriteTime, latestManualFileInfo.LastWriteTime);
                DateTime tmp = (result <= 0) ? latestManualFileInfo.LastWriteTime : latestFileInfo.LastWriteTime;
                LatestExportDateTime = tmp.ToString("dddd dd MMMM yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-GB"));
            }
            if(LatestExportDateTime == "" && latestFileInfo != null)
            {
                LatestExportDateTime = latestFileInfo.CreationTime.ToString("dddd dd MMMM yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-GB"));
            }
            if (LatestExportDateTime == "" && latestManualFileInfo != null)
            {
                LatestExportDateTime = latestManualFileInfo.LastWriteTime.ToString("dddd dd MMMM yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-GB"));
            }

            HistoryExportDateTimeLbl.Text = $"{LatestExportDateTime}";
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
            try
            {
                UpdateAutotintVersion();
            }
            catch (Exception ex)
            {
                MessageBoxResult exInitMsgbox = System.Windows.MessageBox.Show($"{ex.Message}", "", MessageBoxButton.OK);
            }
        }

        //private async void UpdateAutotintVersion()
        private async Task UpdateAutotintVersion()
        {
            string program_data_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            if (File.Exists($"{program_data_path}\\tmp\\network_require.tmp"))
            {
                Logger.Info("Exit and Waiting UpdateAutotintVersion because of file network_require.tmp isExist");
                waitingUpdateAutotintVersion = true;
                lblDatabaseCheckVal.Text = "Waiting ...";
                lblDatabaseVersionText.Text = "Waiting ...";
                return;
            }
            try
            {
                button1.Enabled = false;
                button1.Text = "Checking ...";

                //Check the status is running ?
                waitingUpdateAutotintVersion = false;
                File.Create($"{program_data_path}\\tmp\\dbupdate_client_checked.tmp").Dispose();
                if (File.Exists($"{program_data_path}\\tmp\\dbupdate_running.tmp"))
                {
                    //MessageBoxResult AlertMessageBox2 = System.Windows.MessageBox.Show($"Another Database update process is running please wait for a while and try again", "Message", MessageBoxButton.OK);
                    button1.Enabled = true;
                    button1.Text = "Check for updates";
                    File.Delete($"{program_data_path}\\tmp\\dbupdate_client_checked.tmp");
                    return;
                }
                button1.Text = "Running ...";
                string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
                string str_response = await APIHelper.GetAutoTintVersion(client, auto_tint_id);
                File.Create($"{program_data_path}\\tmp\\dbupdate_running.tmp").Dispose();
                APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(str_response);

                DateTime startTimeFormate = DateTime.UtcNow;
                TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
                //string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));
                string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-GB"));

                if (response.statusCode == 200)
                {
                    AutoTintWithId result = JsonConvert.DeserializeObject<AutoTintWithId>(response.message, Jsonettings);
                    if (result.pos_setting == null)
                    {
                        System.Windows.Forms.MessageBox.Show($"Message : Pos_Setting doesn't have data please check with admin", "Error", MessageBoxButtons.OK);
                        File.Delete($"{program_data_path}\\tmp\\dbupdate_running.tmp");;
                        return;
                    }

                    lblDatabaseCheckVal.Text = ICTDateTimeText;
                    PrismaProLatestVersion checkVersion = new PrismaProLatestVersion();
                    //Check the server for newer version.
                    checkVersion = await APIHelper.GetDBLatestVersion(client, result.pos_setting.id, auto_tint_id);


                    Logger.Info($"Successful on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");

                    var shouldDownloadNewDB = (result.pos_setting_version == null) ? true : (result.pos_setting_version.id < checkVersion.id);
                    if (shouldDownloadNewDB)
                    //if (true)
                    {
                        Logger.Info($"The pos setting version is different : {result.pos_setting_version.id} < {checkVersion.id}");
                        //(Oct 07 2021 Requirement) - Display the client with confirm to update dialog if it's minimize and close (if not require to update not open ??)
                        if (minimizedToTray)
                        {
                            Logger.Info($"Client is in minimized state bring it up");
                            //notifyIcon.Visible = true;
                            this.Show();
                            this.WindowState = FormWindowState.Normal;
                            minimizedToTray = false;
                        }
                        else
                        {
                            Logger.Info($"Client is not in minimized state bring it to front");
                            WinApi.ShowToFront(this.Handle);
                        }
                        //    //Goto download
                        System.Windows.Forms.MessageBox.Show($"The Database is not the latest version \n Current : {result.pos_setting_version?.number} \n Lastest : {checkVersion.number} \n System will continue Download update automatically","Update database version", MessageBoxButtons.OK);
                        Logger.Info($"User's confirmed version download dialog box");

                        string downloadURI = $"{checkVersion.file}";
                        string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                        string tmp_path = $"{path}\\tmp";
                        if (!Directory.Exists(tmp_path))
                        {
                            Directory.CreateDirectory(tmp_path);
                        }
                        String[] URIArray = downloadURI.Split('/');
                        if (!APIHelper.APIConnectionCheck(3, 30)) throw new Exception("Internet Connection Error");
                        Logger.Info($"Internet connection OK the download will be continue.");
                        WebClient webClient = new WebClient();
                        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompletedHandler);
                        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                        //webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompletedHandler);
                        webClient.QueryString.Add("fileName", $"{URIArray[URIArray.Length - 1]}");
                        webClient.QueryString.Add("newVersion", $"{checkVersion.number}");
                        webClient.QueryString.Add("pos_setting_version_id", $"{checkVersion.id}");
                        webClient.DownloadFileAsync(new Uri(downloadURI), $"{tmp_path}\\{URIArray[URIArray.Length - 1]}");//$"{database_path}\\{URIArray[URIArray.Length-1]}");
                                                                                                                //Update to API about new version of database


                    }
                    else
                    {
                        //Set version label
                        lblDatabaseVersionText.Text = (result.pos_setting_version == null) ? "No data" : $"{result.pos_setting_version?.number}";
                        System.Windows.Forms.MessageBox.Show($"Database is up to date", "Message", MessageBoxButtons.OK);
                        File.Delete($"{program_data_path}\\tmp\\dbupdate_running.tmp");
                    }

                }
                else
                {
                    File.Delete($"{program_data_path}\\tmp\\dbupdate_running.tmp");
                    //Set version label
                    lblDatabaseVersionText.Text = "Not Found";
                    lblDatabaseCheckVal.Text = ICTDateTimeText;
                    Logger.Error($"Error on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
                    System.Windows.Forms.MessageBox.Show($"Database Version Check\nStatus Code : {response.statusCode} \nMessage : {response.message}", "Error", MessageBoxButtons.OK);
                }
                button1.Enabled = true;
                button1.Text = "Check for updates";
            }catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Message : {ex.Message}", "Error", MessageBoxButtons.OK);
                Logger.Error($"Exception on update Autotint Database  Message :  {ex.Message}");
                File.Delete($"{program_data_path}\\tmp\\dbupdate_running.tmp");
            }
            
        }
        private void enableEdit()
        {
            tbxShopDispenVal.Enabled = true;
            btnCheckShopID.Enabled = true;
            databaseLocationTextbox.Enabled = true;
            btnDatabaseSelect.Enabled = true;
            posHistoryLocationTextBox.Enabled = true;
            btnSeletectHistoryCVS.Enabled = true;
            SaveInputData.Text = "Save";
        }

        private void disableEdit()
        {
            tbxShopDispenVal.Enabled = false;
            btnCheckShopID.Enabled = false;
            databaseLocationTextbox.Enabled = false;
            btnDatabaseSelect.Enabled = false;
            posHistoryLocationTextBox.Enabled = false;
            btnSeletectHistoryCVS.Enabled = false;
            SaveInputData.Text = "Edit";
        }
        private void SaveInputData_Click(object sender, EventArgs e)
        {
            if (SaveInputData.Text == "Edit")
            {
                using (var form = new PasswordInputForm())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        enableEdit();
                    }
                    if (result == DialogResult.Cancel) {
                        disableEdit();
                    }
                }
                return;
            }


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
                string base_url_tmp = ManageConfig.ReadGlobalConfig("base_url");
                if (base_url_tmp == null)
                {
                    ManageConfig.WriteGlobalConfig("base_url", "http://49.229.21.7");
                }
                else
                {
                    ManageConfig.WriteGlobalConfig("base_url", base_url_tmp);
                }
                disableEdit();
                System.Windows.Forms.MessageBox.Show("Configuration saved", "Success", MessageBoxButtons.OK);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please fill all the form before save", "Error", MessageBoxButtons.OK);
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
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            if (File.Exists($"{path}\\tmp\\network_require.tmp"))
            {
                return;
            }
            var instance = new FileOperationLibrary();

            //Thread progressThread = new Thread(delegate ()
            //{
            //    LoadingForm progress = new LoadingForm();
            //    progress.ShowDialog();
            //});

            //progressThread.Start();
            btnExport1.Text = "Checking ...";
            //Check the status is running ?
            if (File.Exists($"{path}\\tmp\\running.tmp"))
            {
                System.Windows.Forms.MessageBox.Show($"Another Tranfer process is running please wait for a while and try again", "Message", MessageBoxButtons.OK);
                btnExport1.Text = "Upload POS history";
                return;
            }
            btnExport1.Text = "Transfering ...";
            btnExport1.Enabled = false;
            try
            {
                var result = await instance.StartOperation();
                APIHelperResponse res = JsonConvert.DeserializeObject<APIHelperResponse>(result);

                var utcTime = DateTime.UtcNow;
                var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
                logMaskAsDoneDate("" + actualTime);

                //progressThread.Abort();
                //CheckLastestUploadDateTime();
                System.Windows.Forms.MessageBox.Show($"Manual Upload Finish\n{res.message}", "Message", MessageBoxButtons.OK);
            }catch(Exception ex)
            {
                Logger.Error(ex, "Exception on " + ex.ToString());
                System.Windows.Forms.MessageBox.Show($"Internal Error {ex.Message}", "Exception", MessageBoxButtons.OK);
                File.Delete($"{path}\\tmp\\running.tmp");
            }
            
            
            btnExport1.Text = "Upload POS history";
            btnExport1.Enabled = true;
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
            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompletedHandler);
            //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            //webClient.DownloadFileAsync(new Uri("http://49.229.21.7/files/settings/Tint_On_Shop_TuwDkNh.SDF"), @"E:\Tutorial\db_location\Tint_On_Shop_TuwDkNh.SDF");
            //string path = @"E:\Tutorial\csv_history\json_log\full_dispense_log_4_11_2015.json";
            //var instance = new FileOperationLibrary();
            ////instance.convertToBIDataNew(path);
            //instance.UpdateAutotintVersion();
            try
            {
                //string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                //string tmp_path = $"{path}\\tmp";
                //DownloadHelper downloadHelper = new DownloadHelper("http://49.229.21.7/files/settings/Tint_On_Shop_TuwDkNh.SDF",
                //    @"E:\Tutorial\db_location",
                //    @"C:\TOA\Temp");
                ////$"{path}\\tmp");
                //downloadHelper.StartDownload();
                //lblDatabaseVersionText.Text = "No Information";
                //lblDatabaseCheckVal.Text = "No Information";
                string program_data_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                File.Delete($"{program_data_path}\\tmp\\dbupdate_running.tmp");
                File.Delete($"{program_data_path}\\tmp\\dbupdate_client_checked.tmp"); 
                Console.WriteLine("btnDownloadUpdate_Click clicked");
                UpdateAutotintVersion();
            }
            catch(Exception ex)
            {
                Logger.Error($"Exception on btnDownloadUpdate_Click  Message :  {ex.Message}");
            }
            
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar1.Visible = true;
            //progressBar1.Value = e.ProgressPercentage;
            try
            {
                string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
                var jsonData = new ProgressCounter() { total_file = 0, complete_counter = e.ProgressPercentage, status = "Download DB File" };
                File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
            }catch(Exception ex)
            {
                Logger.Error($"Exception on update Autotint Database ProgressChanged Message :  {ex.Message}");
            }
           
        }
        private int _retryCount = 0;
        private async void downloadCompletedHandler(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                Boolean isSuccess = false;
                while (isSuccess == false)
                {
                    isSuccess = await DownloadCompleteActionAsync(sender, e);
                }
                //Confirm after move file
                string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                string downLoadFileName = ((System.Net.WebClient)(sender)).QueryString["fileName"];
                string database_path = ManageConfig.ReadGlobalConfig("database_path");
                Boolean isComplete = File.Exists($"{database_path}\\{downLoadFileName}");
                if (!isComplete)
                {
                    System.Windows.Forms.MessageBox.Show("Downloaded file is corrupt !!!!! \nPlease confirm the dialog to download again", "Error");
                    Logger.Error($"Download Fail on update Autotint Database filename {downLoadFileName}");
                    File.Delete($"{path}\\tmp\\dbupdate_client_checked.tmp");
                }
                else
                {
                    //Update to API about new version of database
                    string pos_setting_version_id = ((System.Net.WebClient)(sender)).QueryString["pos_setting_version_id"];
                    string data = @"
                    {
                    ""pos_setting_version_id"": " + pos_setting_version_id + @"
                    }
                    ";
                    string newVersion = ((System.Net.WebClient)(sender)).QueryString["newVersion"];
                    Logger.Info($"Download Successful continue update version {newVersion} to /auto_tint/{auto_tint_id}/pos_update");
                    dynamic prima_pro_version_response = await APIHelper.RequestPut(client, $"/auto_tint/{auto_tint_id}/pos_update", data, auto_tint_id);
                    //Update version after complete
                    lblDatabaseVersionText.Text = $"{newVersion}";
                    System.Windows.Forms.MessageBox.Show("Download completed! \nDatabase is up to date");
                    Logger.Info($"Download {downLoadFileName} update succesful at {database_path}\\{downLoadFileName}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception on update Autotint Database downloadCompletedHandler Message :  {ex.Message}");
            }

        }

        private async Task<bool> DownloadCompleteActionAsync(object sender, AsyncCompletedEventArgs e)
        {
            //temp folder
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            string tmp_path = $"{path}\\tmp";
            try
            {
                if (File.Exists($"{path}\\tmp\\dbupdate_running.tmp"))
                {
                    File.Delete($"{path}\\tmp\\dbupdate_running.tmp");
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            //Move file to database path
            string downLoadFileName = ((System.Net.WebClient)(sender)).QueryString["fileName"];
            string database_path = ManageConfig.ReadGlobalConfig("database_path");
            try
            {
                
                if (File.Exists($"{database_path}\\{downLoadFileName}"))
                {
                    File.Delete($"{database_path}\\{downLoadFileName}");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            try
            {
                File.Move($"{tmp_path}\\{downLoadFileName}", $"{database_path}\\{downLoadFileName}");
            }
            catch (Exception ex)
            {
                return false;
            }
            progressBar1.Visible = false;
            try
            {
                if (File.Exists($"{path}\\tmp\\lib_running_log.json"))
                {
                    File.Delete($"{path}\\tmp\\lib_running_log.json");
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            File.Create($"{path}\\tmp\\dbupdate_client_checked.tmp").Dispose();
            Logger.Info($"dbupdate_client_checked.tmp created because of download and move file is successful");
            return true;
        }

        #region Windows_Controller
        private void btnMinToTray_Click(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            MinimizeToTray();
        }
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                ShowWindow();
            }
            base.WndProc(ref message);
        }
        private void btnMinToTray_Click(object sender, EventArgs e)
        {
            // Tie this function to a button on your main form that will minimize your
            // application to the notification icon area (aka system tray).
            MinimizeToTray();
        }
        void MinimizeToTray()
        {
            //notifyIcon = new NotifyIcon();
            //notifyIcon.Click += new EventHandler(NotifyIconClick);
            //notifyIcon.DoubleClick += new EventHandler(NotifyIconClick);
            //notifyIcon.Icon = Resources.SystemTrayApp;
            //notifyIcon.Text = ProgramInfo.AssemblyTitle;
            //notifyIcon.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            minimizedToTray = true;
            disableEdit();
        }
        public void ShowWindow()
        {
            if (minimizedToTray)
            {
                //notifyIcon.Visible = true;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                minimizedToTray = false;
                UpdateAutotintVersion();
            }
            else
            {
                WinApi.ShowToFront(this.Handle);
            }
        }
        void NotifyIconClick(Object sender, System.EventArgs e)
        {
            ShowWindow();
        }

        #endregion
    }
}
