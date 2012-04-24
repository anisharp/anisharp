using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace AniSharp.API
{
    class APIConnection
    {
        private Thread senderThread;
        private UdpClient udpClient;
        private BlockingCollection<String> outQueue = new BlockingCollection<String>();
        private String session;

        private void senderThreadInit()
        {
            while (true)
            {
                String toSend = outQueue.Take();
                byte[] bytes = Encoding.ASCII.GetBytes(toSend+"&s="+session);
                getConnection().Send(bytes, bytes.Length);

                // IPEndPoint object will allow us to read datagrams sent from any source.
                // IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                // Byte[] receiveBytes = getConnection().Receive(ref RemoteIpEndPoint);
                // string returnData = Encoding.ASCII.GetString(receiveBytes);

                Thread.Sleep(2000);
            }
        }

        public void establishConnection(String username, String password)
        {
            try
            {   
                udpClient = new UdpClient(AniSharp.Properties.Settings.Default.LocalPort);
                udpClient.Connect(AniSharp.Properties.Settings.Default.Address, AniSharp.Properties.Settings.Default.RemotePort);

                byte[] bytes = Encoding.ASCII.GetBytes("AUTH user="+username+"&pass="+password+"&protover="+AniSharp.Properties.Settings.Default.ApiVersion+"&client="+AniSharp.Properties.Settings.Default.ClientName+"&clientver="+AniSharp.Properties.Settings.Default.Version);
                udpClient.Send(bytes, bytes.Length);

                // IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = getConnection().Receive(ref RemoteIpEndPoint);
                String returnData = Encoding.ASCII.GetString(receiveBytes);
                MessageBox.Show(returnData);
                this.session = returnData.Split(' ')[1];

                senderThread = new Thread(senderThreadInit);
                senderThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private UdpClient getConnection()
        {
            return this.udpClient;
        }

        public void closeUDPClient()
        {
            senderThread.Interrupt();
            udpClient.Close();
        }
    }
}
