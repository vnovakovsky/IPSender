using HttpClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class HttpClientTest
    {
        [TestMethod]
        public void UpdateNetworkParameters()
        {
            try
            { 
                RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        private static async Task RunAsync()
        {
            NetworkParameters networkParameters = new NetworkParameters
            {
                IP = "192.168.0.1",
                HostName = "myFQDN",
            };
            await HttpClient.HttpClient.RunAsync(networkParameters);
        }
    }
}
