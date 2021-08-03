using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
    class WebClientUtility : WebClient
    {
        public int Timeout { get; set; }

        public WebClientUtility() : this(60000) { }

        public WebClientUtility(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
            }
            return request;
        }
    }

    //
    public class DownloadHelper : IDisposable
    {
        private WebClientUtility _webClient;
        private string _downloadUrl;
        private string _savePath;
        private string _tmpPath;
        private int _retryCount;

        public DownloadHelper(string downloadUrl, string savePath, string tmpPath)
        {
            _downloadUrl = downloadUrl;
            String[] URIArray = downloadUrl.Split('/');
            string filename = URIArray[URIArray.Length - 1];
            _savePath = savePath;
            _tmpPath = $"{tmpPath}\\{filename}";

            _webClient = new WebClientUtility();
            
            _webClient.DownloadFileCompleted += ClientOnDownloadFileCompleted;
            _webClient.DownloadProgressChanged += ClientOnProgressChange;
            _webClient.QueryString.Add("fileName", $"{filename}");
        }


        public void StartDownload()
        {
            string program_data_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            File.Create($"{program_data_path}\\tmp\\dbupdate_running.tmp").Dispose();
            _webClient.DownloadFileAsync(new Uri(_downloadUrl), _tmpPath);
        }

        private void ClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            if (e.Error != null)
            {
                _retryCount++;

                if (_retryCount < 3)
                {
                    _webClient.DownloadFileAsync(new Uri(_downloadUrl), _tmpPath);
                }
                else
                {
                    Console.WriteLine(e.Error.Message);
                    Console.WriteLine(e.Error.InnerException.Message);
                }
            }
            else
            {
                _retryCount = 0;
                //string tmp_path = $"{path}\\tmp";
                File.Delete($"{path}\\tmp\\dbupdate_running.tmp");
                string downLoadFileName = ((System.Net.WebClient)(sender)).QueryString["fileName"];
                //string database_path = ManageConfig.ReadGlobalConfig("database_path");
                if (File.Exists($"{_savePath}\\{downLoadFileName}"))
                {
                    File.Delete($"{_savePath}\\{downLoadFileName}");
                }
                File.Move($"{_tmpPath}", $"{_savePath}\\{downLoadFileName}");
                File.Delete($"{path}\\tmp\\lib_running_log.json");
                Console.WriteLine($"successfully download: # {_downloadUrl}  to  # {_savePath}");
            }
        }

        private void ClientOnProgressChange(object sender, DownloadProgressChangedEventArgs e)
        {
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
            var jsonData = new ProgressCounter() { total_file = 0, complete_counter = e.ProgressPercentage, status = "Download DB File" };
            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
        }
        public void Dispose()
        {
            _webClient.Dispose();
        }
    }
}
