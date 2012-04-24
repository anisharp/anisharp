using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;

namespace AniSharp.API
{
    class APIConnection
    {
        private byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }

        private string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }

        private BlockingCollection<String> outQueue = new BlockingCollection<String>();
        private void senderThreadInit()
        {
            while (true)
            {
                String toSend = outQueue.Take();
                byte[] bytes = StringToByteArray(toSend);
                getConnection().Send(bytes, bytes.Length);

                Thread.Sleep(2000);
            }
        }

        private Thread senderThread;
        
        private UdpClient udpClient;

        public void establishConnection()
        {
            try
            {   
                udpClient = new UdpClient(AniSharp.Properties.Settings.Default.LocalPort);
                udpClient.Connect(AniSharp.Properties.Settings.Default.Address, AniSharp.Properties.Settings.Default.RemotePort);

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
    }
}
