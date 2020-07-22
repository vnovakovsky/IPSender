using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


namespace IPSender
{

    public static class IPResolver
    {
        public static string GetIP()
        {
            string localIP;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    var address = Dns.GetHostAddresses("www.google.com")[0];
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
    }
}
