using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.ComponentModel;
using System.Text;
using NLog;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
namespace AutoTintLibrary
{
    public class APIHelper
    {
        public static string baseURL = (File.Exists(@"C:\ProgramData\TOA_Autotint\config.json")) ? (ManageConfig.ReadGlobalConfig("base_url") != null) ? ManageConfig.ReadGlobalConfig("base_url") : "http://49.229.21.7" : "http://49.229.21.7";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static RestClient init()
        {
            clearInvalidConfigFile();
            var client = new RestClient();
            client.Encoding = Encoding.UTF8;
            //client.AddDefaultHeader("Content-Type", "text/html;charset=gb2312");
            return client;
        }
        //Creating the extern function...  
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static void clearInvalidConfigFile()
        {
            if (File.Exists(@"C:\ProgramData\TOA_Autotint\config.json"))
            {
                String base_url = ManageConfig.ReadGlobalConfig("base_url");
                String path = ManageConfig.ReadGlobalConfig("base_url");
                if (base_url == null && path == null)
                {
                    File.Delete(@"C:\ProgramData\TOA_Autotint\config.json");
                }
            }
        }

        public static bool APIConnectionCheck(int retry_round,int interval)
        {
            int status_code;
            int connection_retry = 1;
            bool isConnected = false;
            bool IsServerReply = false;
            var ping = new Ping();
            while (connection_retry <= retry_round)
            {
                isConnected = InternetGetConnectedState(out status_code, 0);
                try
                {
                    //Comment this section out until find the better way to get the API server status
                   // var pingReply = ping.Send("http://49.229.21.7/docs/", 30 * 1000); // 1 minute time out (in ms)
                    IsServerReply = true;
                }
                catch(Exception ex)
                {
                    IsServerReply = false;
                }
                
                Logger.Info($"Internet status isConnected: {isConnected} Connection Flag : {status_code}");
                connection_retry++;
                if (!isConnected || !IsServerReply)
                {
                    Logger.Error($"Internet status isConnected:{isConnected} Connection Flag : {status_code}");
                    Logger.Info($"Retrying in {interval} seconds ...");
                    System.Threading.Thread.Sleep(interval*1000);
                }
                else
                {
                    return true;
                }
            }
            if (connection_retry > retry_round)
            {
                return false;
            }
            return true;
        }
        public static async Task<string> RequestGet(RestClient client, string url,string auto_tint_id)
        {
            IRestResponse response = new RestResponse();
            try
            {
                var request = new RestRequest($"{baseURL}{url}", Method.GET);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Client-Id", auto_tint_id);
                //request.Parameters.Clear();

                if (!APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Console.WriteLine(response.Content);

                //log result.Content;
                if (!url.Contains("/base/?page"))
                {
                    Logger.Info($"Param {url},{auto_tint_id}:Response Status {response.StatusCode} | Content {response.Content}");
                }
                else
                {
                    Logger.Info($"Param {url},{auto_tint_id}:Response Status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called RequestGet {url} : {ex}");
                throw new Exception($"Exception when called RequestGet : {ex.Message}");
            }

            try { 
                dynamic dddd = JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
            }catch(Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called JsonConvert {url} : {ex.ToString()}");
                throw new Exception($"Exception when called RequestGet : {ex.Message}");
            }

            return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
        }

        public static async Task<string> RequestPut(RestClient client, string url,string Jsondata,string auto_tint_id)
        {
            IRestResponse response = new RestResponse();
            try
            {
                var request = new RestRequest($"{baseURL}{url}/", Method.PUT);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Client-Id", auto_tint_id);
                Order order = JsonConvert.DeserializeObject<Order>(Jsondata);

                request.AddJsonBody(new { pos_setting_version_id = order.pos_setting_version_id });
                if (!APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Console.WriteLine(response.Content);
                Logger.Info($"Param {url}, {Jsondata}, {auto_tint_id}:Response Status {response.StatusCode} | Content {response.Content}");
                //return result.Content;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called RequestPut {url} : {ex.ToString()}");
                Logger.Error($"Exception when called RequestPut Data {Jsondata}");
                throw new Exception($"Exception when called RequestPut : {ex.Message}");

            }
            return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
        }

        public static async Task<string> GetLatestDispenseRecord(RestClient client, string auto_tint_id, string url)
        {
            IRestResponse response = new RestResponse();
            try
            {
                var request = new RestRequest($"{baseURL}{url}", Method.GET).AddParameter("auto_tint_id", auto_tint_id);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Client-Id", auto_tint_id);
                if (!APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Console.WriteLine(response.Content);
                //return result.Content;
                Logger.Info($"Param {url}, {auto_tint_id}:Response Status {response.StatusCode} | Content {response.Content}");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called GetLatestDispenseRecord {url} : {ex.ToString()}");
                throw new Exception($"Exception when called GetLatestDispenseRecord : {ex.Message}");
            }

            return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
        }


        public static async Task<string> UploadFile(RestClient client,string method, string file_path,string auto_tint_id)
        {
            IRestResponse response = new RestResponse();
            try
            {
                var request = new RestRequest($"{baseURL}/{method}/", Method.POST);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Client-Id", auto_tint_id);
                string streamFile = File.ReadAllText(file_path);
                request.AddParameter("data", streamFile, ParameterType.RequestBody);
                if (!APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Logger.Info($"Param {file_path}, {auto_tint_id}:Response Status {response.StatusCode} | Content {response.Content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called UploadFile {method} : {ex.ToString()}");
                Logger.Error($"Exception when called UploadFile {file_path}");
                throw new Exception($"Exception when called UploadFile : {ex.Message}");
            }

            return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
        }
        public static async Task<string> GetAutoTintVersion(RestClient client, string auto_tint_id)
        {
            IRestResponse response = new RestResponse();
            try
            {
                //auto_tint_id = "12345678AT01";
                var request = new RestRequest(baseURL + "/auto_tint/" + auto_tint_id, Method.GET);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Client-Id", auto_tint_id);
                //request.Parameters.Clear();
                if (!APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Logger.Info($"Param {auto_tint_id}:Response Status {response.StatusCode} | Content {response.Content}");
                //Console.WriteLine(result.Content);
                //return JsonConvert.DeserializeObject<AutoTintWithId>(result.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called GetAutoTintVersion of {auto_tint_id} : {ex.ToString()}");
                throw new Exception($"Exception when called GetAutoTintVersion of {auto_tint_id} : {ex.Message}");
                //return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });

            }
            //return JsonConvert.DeserializeObject<AutoTintWithId>(response.Content);
            return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
        }

        public static async Task<PrismaProLatestVersion> GetDBLatestVersion(RestClient client, int pos_setting_id,string auto_tint_id)
        {
            IRestResponse response = new RestResponse();
            try
            {
                var request = new RestRequest(baseURL + "/prisma_pro/" + pos_setting_id + "/latest_version", Method.GET);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Client-Id", auto_tint_id);
                //request.Parameters.Clear();
                if (!APIConnectionCheck(3, 30)) throw new Exception("Connection error");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                //Console.WriteLine(result.Content);
                PrismaProLatestVersion checkVersion = JsonConvert.DeserializeObject<PrismaProLatestVersion>(response.Content);
                Logger.Info($"Param {pos_setting_id} , {auto_tint_id}:Response Status {response.StatusCode} | Content {response.Content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);
                Logger.Error($"Exception when called GetDBLatestVersion of {pos_setting_id} : {ex.ToString()}");
                throw new Exception($"Exception when called GetDBLatestVersion of {auto_tint_id} : {ex.Message}");
            }
            return JsonConvert.DeserializeObject<PrismaProLatestVersion>(response.Content);
        }
    }

    internal class Order
    {
        public string pos_setting_version_id { get; set; }
        public string value { get; set; }
    }
}
