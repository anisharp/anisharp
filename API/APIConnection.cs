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
        private Thread receiverThread;
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

                senderThread = new Thread(senderThreadInit);
                senderThread.Start();
                receiverThread = new Thread(receiverThreadStart);
                receiverThread.Start();


                String loginQ = "AUTH user=" + username + "&pass=" + password + "&protover=" + AniSharp.Properties.Settings.Default.ApiVersion + "&client=" + AniSharp.Properties.Settings.Default.ClientName + "&clientver=" + AniSharp.Properties.Settings.Default.Version;
                String loginA = query(loginQ);

                MessageBox.Show(loginA);
                this.session = loginA.Split(' ')[2];

               
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

        private Dictionary<String, String> results = new Dictionary<string, string>();

        public String query(String queryS)
        {
            String tag = generateTag();
            queryS += "&tag=" + tag;
            outQueue.Add(queryS);
            while (!results.ContainsKey(tag))
                Monitor.Wait(results);

            return results[tag];
        }

        //TODO: uniquify TAGS!

        private static Random RND = new Random();
        private static String TAG_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static int TAG_LEN = 5;
        private static String generateTag()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < TAG_LEN; i++)
            {
                sb.Append(TAG_CHARS.ElementAt(RND.Next(TAG_CHARS.Length)));
            }
            return sb.ToString();
        }

        public void receiverThreadStart()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                Byte[] receiveBytes = getConnection().Receive(ref RemoteIpEndPoint);
                String returnData = Encoding.ASCII.GetString(receiveBytes);

                String tag = returnData.Split(' ')[0];

                results.Add(tag, returnData);
                Monitor.PulseAll(results);
            }
        }
    }
}
