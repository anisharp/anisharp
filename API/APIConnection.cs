using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace AniSharp.API
{
    class APIConnection
    {
        private UdpClient udpClient;

        public void establishConnection()
        {
            try
            {   
                udpClient = new UdpClient(AniSharp.Properties.Settings.Default.LocalPort);
                udpClient.Connect(AniSharp.Properties.Settings.Default.Address, AniSharp.Properties.Settings.Default.RemotePort);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public UdpClient getConnection()
        {
            return this.udpClient;
        }

        public void closeUDPClient()
        {
            udpClient.Close();
        }
    }
}
