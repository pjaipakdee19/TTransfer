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
using System.Threading;
using System.Net;
using System.Net.Sockets;

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
        public async void startUpProcess()
        {
            
            var utcTime = DateTime.UtcNow;
            var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
            string programdata_path = ManageConfig.ReadConfig("programdata_log_path");
            var instance = new FileOperationLibrary();
            if (!APIHelper.APIConnectionCheck(3, 30)) throw new Exception("Connection error");
            //Download database
            var baseDBDLresult = await instance.downloadBaseDB();
            if (!baseDBDLresult) Logger.Error("[AutoStart]downloadBaseDB error please check the lib log");
            bool skipStartProcess = false;
            //Check the time with server time.
            DateTime NetworkTime = GetNetworkTime();
            Logger.Info("[AutoStart]Current ServerTime from time.windows.com " + NetworkTime);
            //Check current computer time.
            DateTime localPCDate = DateTime.Now;
            Logger.Info("[AutoStart]Current Client Time " + localPCDate);
            TimeSpan ts = localPCDate - NetworkTime;
            var Diffmin = Math.Abs(ts.TotalMinutes);
            Logger.Info("[AutoStart]No. of Minutes (Difference) = {0}", Diffmin);
            if (Diffmin > 1)
            {
                Logger.Error($"Time is different between server > 1 (Actual : {Diffmin} min) min please check PC timezone setting.");
                Logger.Info("AutoStart process will not start working please re-configuration the computer's time");
                skipStartProcess = true;
            }
           


            if (!skipStartProcess)
            {
                Logger.Info($"[AutoStart] Start transfer operation when PC turn on {actualTime} !!!");
                bool gotException = false;
                
                //Lock the manual process
                File.Create($"{programdata_path}\\tmp\\running.tmp").Dispose();
                File.Create($"{programdata_path}\\tmp\\dbupdate_running.tmp").Dispose();
                try
                {
                    var result = await instance.StartOperation();
                    APIHelperResponse res = JsonConvert.DeserializeObject<APIHelperResponse>(result);
                    if (res.statusCode == 200) Logger.Info($"[AutoStart] Start transfer status {res.statusCode} message {res.message}");
                    if (res.statusCode == 500) Logger.Error($"[AutoStart] Start transfer status {res.statusCode} message {res.message}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"[AutoStart] Start transfer Exception {ex.Message}");
                    gotException = true;
                }

                Logger.Info($"[AutoStart] Transfer operation when PC turn on Finish on {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ictZone)} !!!");
                Logger.Info($"[AutoStart] Start checking and update database at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ictZone)} !!!");
                try
                {
                    var isUpdateOkay = await instance.UpdateAutotintVersion();
                    if (isUpdateOkay)
                    {
                        Logger.Info($"[AutoStart] Update the database done at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ictZone)} !!!");
                    }
                    else
                    {
                        Logger.Error($"[AutoStart] Update the database got error at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ictZone)}, Please check the lib log");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[AutoStart] Update the database got Exception {ex.Message}");
                    gotException = true;
                }

                if (gotException)
                {
                    //Delete the file for prevent deadlock on client side
                    File.Delete($"{programdata_path}\\tmp\\running.tmp");
                    File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
                }
            }
            

            Logger.Info("[Service] Start timer");
            System.Timers.Timer timScheduledTask = new System.Timers.Timer();
            timScheduledTask.Enabled = true;
            timScheduledTask.Interval = 60 * 1000;
            timScheduledTask.Elapsed += new System.Timers.ElapsedEventHandler(timeScheduledTask_Elapsed);
        }
        protected override async void OnStart(string[] args)
        {

            var utcTime = DateTime.UtcNow;
            var ictZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var actualTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ictZone);
            //string programdata_path = ConfigurationManager.AppSettings.Get("programdata_log_path");
            string programdata_path = ManageConfig.ReadConfig("programdata_log_path");
            DirectoryInfo programdata_info = new DirectoryInfo(programdata_path);

            CreateDirectoryIfNotExist($"{programdata_path}\\tmp");
            //Clear the tmp file 
            File.Delete($"{programdata_path}\\tmp\\service_not_start.tmp");
            File.Delete($"{programdata_path}\\tmp\\running.tmp");
            File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
            File.Delete($"{programdata_path}\\tmp\\dbupdate_version_check.tmp");
            File.Delete($"{programdata_path}\\tmp\\dbupdate_client_checked.tmp");
            try
            {
                
                Thread startupThread = new Thread(new ThreadStart(startUpProcess));
                startupThread.Start();

            }catch(Exception ex)
            {
                Logger.Error(ex, "Exception on " + ex.ToString());
                File.Delete($"{programdata_path}\\tmp\\running.tmp");
                File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
            }
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
                    bool skipTimerProcess = false;
                    //Check the time with server time.
                    if (!APIHelper.APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                    DateTime NetworkTime = GetNetworkTime();
                    Logger.Info("[TimerProcess]Current ServerTime from time.windows.com " + NetworkTime);
                    //Check current computer time.
                    DateTime localPCDate = DateTime.Now;
                    Logger.Info("[TimerProcess]Current Client Time " + localPCDate);
                    TimeSpan ts = localPCDate - NetworkTime;
                    var Diffmin = Math.Abs(ts.TotalMinutes);
                    Logger.Info("[TimerProcess]No. of Minutes (Difference) = {0}", Diffmin);
                    if (Diffmin > 1)
                    {
                        Logger.Error($"Time is different between server > 1 (Actual : {Diffmin} min) min please check PC timezone setting.");
                        Logger.Info("TimerProcess will not start working please re-configuration the computer's time");
                        skipTimerProcess = true;
                    }

                    if (!skipTimerProcess)
                    {
                        //Go working extract log and send process
                        try
                        {
                            var instance = new FileOperationLibrary();
                            await instance.StartOperation();
                            //Start check for update and download if needed
                            await instance.UpdateAutotintVersion();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Exception on " + ex.ToString());
                        }
                    }
                    logMaskAsDoneDate("" + actualTime);
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

        private void CreateDirectoryIfNotExist(string filepath)
        {
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
        }

        public DateTime GetNetworkTime()
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";
            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
    }
}
