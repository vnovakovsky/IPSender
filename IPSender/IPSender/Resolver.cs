using System.Configuration;
using System.Net;
using System.Net.Sockets;


namespace IPSender
{

    public static class Resolver
    {
        public static string GetIP()
        {
            string localIP;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    string urlToDefineNetworkInterface = ConfigurationManager.AppSettings["URLToDefineNetworkInterface"];
                    var address = Dns.GetHostAddresses(urlToDefineNetworkInterface)[0];
                    socket.Connect(address, 80);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
            }
            else
            {
                localIP = "Network is not available";
            }
            return localIP;
        }
        public static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }
    }
}
