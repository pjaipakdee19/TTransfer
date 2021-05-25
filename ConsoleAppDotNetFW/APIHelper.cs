using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Threading;

namespace ConsoleAppDotNetFW
{
    class APIHelper
    {
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
            return JsonConvert.SerializeObject(result.Content);
        }

        [Obsolete]
        public static async Task<string> UploadFile(RestClient client, string url,string file_path)
        {
            var request = new RestRequest(url, Method.POST);
            request.AddFile("file", file_path);
            var cancellationTokenSource = new CancellationTokenSource();
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            IRestResponse result = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            return JsonConvert.SerializeObject(result.Content);
        }
    }
}
