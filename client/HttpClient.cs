using log4net;
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
        private static readonly ILog _log = LogManager.GetLogger("LOGGER");
        static System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();
        static HttpClient()
        {
            string webAPIRecipientURL = ConfigurationManager.AppSettings["WebAPIRecipientURL"];
            _client.BaseAddress = new Uri(webAPIRecipientURL);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region UpdateProductAsync
        static async Task<HttpStatusCode> UpdateProductAsync(NetworkParameters networkParameters)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync(
                $"api/values/{networkParameters.IP}/{networkParameters.HostName}", networkParameters);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                _log.Debug("Params is sent: response.StatusCode = " + response.StatusCode);
            }
            else
            {
                _log.Error("Params is NOT sent: response.StatusCode = " + response.StatusCode);
            }

            return response.StatusCode;
        }
        #endregion

        public static async Task RunAsync(NetworkParameters networkParameters)
        {
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