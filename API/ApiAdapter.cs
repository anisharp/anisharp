﻿
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using AniSharp.API.Transport;

namespace AniSharp.API
{
    /// <summary>
    /// Sends commands to AniDB, and receives their counterparts
    /// </summary>
	class ApiAdapter : Queryable
	{
		private BlockingCollection<String> outQueue = new BlockingCollection<String>();
		private Dictionary<String, String> results = new Dictionary<string, string>();
		private Thread senderThread;
		private Thread receiverThread;

        private UdpAdapter udpadapter;

		public const int WAITING_BETWEEN_PACKETS = 2000;

		public ApiAdapter()
		{
            System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(AniSharp.Properties.Settings.Default.Address);
			udpadapter = new DefaultUdpAdapter(new IPEndPoint(host.AddressList[0], AniSharp.Properties.Settings.Default.RemotePort));

			senderThread = new Thread(senderThreadInit);
			senderThread.Start();
			receiverThread = new Thread(receiverThreadStart);
			receiverThread.Start();
		}

		public void shutdown()
		{
			senderThread.Interrupt();
			receiverThread.Interrupt();

            udpadapter.shutdown();
		}


		private void senderThreadInit()
		{
			try
			{
				while (true)
				{
					String toSend = outQueue.Take();
                    udpadapter.send(toSend);

					Thread.Sleep(WAITING_BETWEEN_PACKETS);
				}
			}
			catch (Exception)
			{
			}
		}
		/// <summary>
		/// sends a command to AniDB. throws InvalidOperationException if session key is not
		/// set, but mandatory
		/// </summary>
		/// <param name="req">The method-request</param>
		/// <returns>The result</returns>
		public ApiAnswer query(ApiRequest req)
		{
			// set a tag
			String tag = generateTag();
			req["tag"] = tag;

			outQueue.Add(req.ToString());
			while (!results.ContainsKey(tag))
				lock (results)
				{
					Monitor.Wait(results);
				}
			String result = results[tag];
			// drop answer from dictionary?
            return ApiAnswer.parse(result);
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
                
				sb.Append(TAG_CHARS[RND.Next(TAG_CHARS.Length)]);
			}
			return sb.ToString();
		}

		private void receiverThreadStart()
		{
			try
			{
				while (true)
				{
                    String returnData = udpadapter.receive();

                    String tag = returnData.Substring(0, TAG_LEN);

                    String strippedData = returnData.Substring(TAG_LEN + 1);

                    results.Add(tag, strippedData);


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
