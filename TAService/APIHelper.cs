using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TAService
{
    class APIHelper
    {
        public static string baseURL = "http://49.229.21.8/poc";
        public static RestClient init()
        {
            var client = new RestClient();
            return client;
        }

        [Obsolete]
        public static async Task<string> RequestGet(RestClient client, string url)
        {
            var request = new RestRequest(url, Method.GET);
            var cancellationTokenSource = new CancellationTokenSource();
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            IRestResponse result = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            Console.WriteLine(result.Content);
            return result.Content;
        }

        [Obsolete]
        public static async Task<string> UploadFile(RestClient client, string url,string file_path)
        {
            try
            {
                var request = new RestRequest(url, Method.POST);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                string streamFile = File.ReadAllText(file_path);
                request.AddParameter("data", streamFile, ParameterType.RequestBody);
                IRestResponse result = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
                
                return JsonConvert.SerializeObject(new { statusCode = result.StatusCode, message = result.Content });
            }catch(Exception ex)
            {
                Console.WriteLine("Call logger about exception "+ex);
                return "Error";
            }
        }

        [Obsolete]
        public static async Task<AutoTintWithId> GetAutoTintVersion(RestClient client,string auto_tint_id)
        {
            var request = new RestRequest(baseURL+ "/auto_tint/" + auto_tint_id, Method.GET);
            var cancellationTokenSource = new CancellationTokenSource();
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            IRestResponse result = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            //Console.WriteLine(result.Content);
            return JsonConvert.DeserializeObject<AutoTintWithId>(result.Content);
        }

        [Obsolete]
        public static async Task<PrismaProLatestVersion> GetDBLatestVersion(RestClient client, int pos_setting_id)
        {
            var request = new RestRequest(baseURL + "/prisma_pro/" + pos_setting_id + "/latest_version", Method.GET);
            var cancellationTokenSource = new CancellationTokenSource();
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            IRestResponse result = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            //Console.WriteLine(result.Content);
            PrismaProLatestVersion checkVersion = JsonConvert.DeserializeObject<PrismaProLatestVersion>(result.Content);
            return JsonConvert.DeserializeObject<PrismaProLatestVersion>(result.Content);
        }
    }
}
