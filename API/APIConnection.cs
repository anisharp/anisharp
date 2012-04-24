using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AniSharp.API
{
    class APIConnection
    {
        private Thread senderThread;
        private Thread receiverThread;
        private UdpClient udpClient;
        private BlockingCollection<String> outQueue = new BlockingCollection<String>();
        private String session = null;
        private Dictionary<String, String> results = new Dictionary<string, string>();

        public const int WAITING_BETWEEN_PACKETS = 2000;

        private void senderThreadInit()
        {
            try
            {
                while (true)
                {
                    String toSend = outQueue.Take();
                    byte[] bytes = Encoding.ASCII.GetBytes(toSend + (session == null ? "" : "&s=" + session));
                    getConnection().Send(bytes, bytes.Length);

                    Thread.Sleep(WAITING_BETWEEN_PACKETS);
                }
            }
            catch (Exception)
            {
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

                ApiRequest loginQ = new ApiRequest("AUTH");
                loginQ.set("user", username)
                    .set("pass", password)
                    .set("protover", AniSharp.Properties.Settings.Default.ApiVersion.ToString())
                    .set("client", AniSharp.Properties.Settings.Default.ClientName)
                    .set("clientver", AniSharp.Properties.Settings.Default.Version.ToString());

               /* String loginQ = "AUTH user=" + username + "&pass=" + password + "&protover=" + 
                    AniSharp.Properties.Settings.Default.ApiVersion + "&client=" +
                    AniSharp.Properties.Settings.Default.ClientName + "&clientver=" +
                    AniSharp.Properties.Settings.Default.Version;*/
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
            ApiRequest logoutQ = new ApiRequest("LOGOUT");
            //String logoutQ = "LOGOUT ";
            String logoutA = query(logoutQ);
            MessageBox.Show(logoutA);
            senderThread.Interrupt();
            receiverThread.Interrupt();
            udpClient.Close();
        }


        /// <summary>
        /// checks, if this MethodCall will result in an unauthorized error
        /// </summary>
        /// <param name="command">The command-name to execute</param>
        private void checkMethodSessionKey(String command)
        {
            //PING, ENCRYPT, ENCODING, AUTH and VERSION are allowed
            if (session != null) return; // session set -- ok

            switch (command)
            {
                case "PING":
                case "ENCRYPT":
                case "ENCODING":
                case "AUTH":
                case "VERSION":
                    return;
                default:
                    throw new System.InvalidOperationException("Session not set");
            }
        }

        /// <summary>
        /// sends a command to AniDB. throws InvalidOperationException if session key is not
        /// set, but mandatory
        /// </summary>
        /// <param name="req">The method-request</param>
        /// <returns>The result</returns>
        public String query(ApiRequest req)
        {
            checkMethodSessionKey(req.Command);

            // set a tag
            String tag = generateTag();
            req["tag"] = tag;

            // transmit session-key
            if (this.session != null)
                req["s"] = this.session;

            outQueue.Add(req.ToString());
            while (!results.ContainsKey(tag))
                lock (results)
                {
                    Monitor.Wait(results);
                }
            return results[tag];
        }

        public String query(String queryS)
        {
            String tag = generateTag();
            queryS += "&tag=" + tag;
            outQueue.Add(queryS);
            while (!results.ContainsKey(tag))
                lock (results)
                {
                    Monitor.Wait(results);
                }
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
            try
            {
                while (true)
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    Byte[] receiveBytes = getConnection().Receive(ref RemoteIpEndPoint);
                    String returnData = Encoding.ASCII.GetString(receiveBytes);

                    String tag = returnData.Split(' ')[0];

                    results.Add(tag, returnData);
                    lock (results)
                    {
                        Monitor.PulseAll(results);
                    };
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
