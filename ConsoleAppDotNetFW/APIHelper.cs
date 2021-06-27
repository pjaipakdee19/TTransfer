using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;

namespace ConsoleAppDotNetFW
{
    class APIHelper
    {
        public static string baseURL = "http://49.229.21.7/dev";
        public static RestClient init()
        {
            var client = new RestClient();
            return client;
        }

        public static async Task<string> RequestGet(RestClient client, string url)
        {
            IRestResponse response = new RestResponse();
            try
            {
                var request = new RestRequest($"{baseURL}{url}", Method.GET);
                var cancellationTokenSource = new CancellationTokenSource();
                request.AddHeader("Accept", "application/json");
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Console.WriteLine(response.Content);
                //return result.Content;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);

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
                response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                Console.WriteLine(response.Content);
                //return result.Content;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Call logger about exception " + ex);

            }

            return JsonConvert.SerializeObject(new { statusCode = response.StatusCode, message = response.Content });
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
    }
}
