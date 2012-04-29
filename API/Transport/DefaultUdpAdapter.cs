using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AniSharp.API.Transport
{
    public class DefaultUdpAdapter : UdpAdapter
    {
        private UdpClient client;

        public DefaultUdpAdapter(IPEndPoint i)
        {
            client = new UdpClient(AniSharp.Properties.Settings.Default.LocalPort);
            client.Connect(i);
        }

        public String receive()
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = client.Receive(ref remote);

            return Encoding.ASCII.GetString(bytes);
        }

        public void send(String s)
        {
            
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            client.Send(bytes, bytes.Length);
        }

        public void shutdown()
        {
            client.Close();
        }
    }
}
