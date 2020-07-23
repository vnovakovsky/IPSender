using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HttpClient
{
    #region NetworkParameters
    public class NetworkParameters
    {
        public string IP { get; set; }
        public string HostName { get; set; }
    }
    #endregion

    public static class HttpClient
    {
        static System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

        #region UpdateProductAsync
        static async Task<HttpStatusCode> UpdateProductAsync(NetworkParameters networkParameters)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/values/{networkParameters.IP}/{networkParameters.HostName}", networkParameters);
            response.EnsureSuccessStatusCode();

            return response.StatusCode;
        }
        #endregion

        static void Main()
        {
            NetworkParameters networkParameters = new NetworkParameters
            {
                IP = "192.168.0.1",
                HostName = "myFQDN",
            };
            RunAsync(networkParameters).GetAwaiter().GetResult();
        }

        public static async Task RunAsync(NetworkParameters networkParameters)
        {
            string webAPIRecipientURL = ConfigurationManager.AppSettings["WebAPIRecipientURL"];
            client.BaseAddress = new Uri(webAPIRecipientURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Update IP, host name
                await UpdateProductAsync(networkParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}