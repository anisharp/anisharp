
using System.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AniSharp.API
{
	class ApiDriver
	{
		private BlockingCollection<String> outQueue = new BlockingCollection<String>();
		private Dictionary<String, String> results = new Dictionary<string, string>();
		private Thread senderThread;
		private Thread receiverThread;
		private UdpClient udpClient;

		public const int WAITING_BETWEEN_PACKETS = 2000;

		public ApiDriver()
		{
		}

		public void connect()
		{
			udpClient = new UdpClient(AniSharp.Properties.Settings.Default.LocalPort);
			udpClient.Connect(AniSharp.Properties.Settings.Default.Address, AniSharp.Properties.Settings.Default.RemotePort);

			senderThread = new Thread(senderThreadInit);
			senderThread.Start();
			receiverThread = new Thread(receiverThreadStart);
			receiverThread.Start();
		}

		public void disconnect()
		{
			senderThread.Interrupt();
			receiverThread.Interrupt();
			udpClient.Close();
		}


		private void senderThreadInit()
		{
			try
			{
				while (true)
				{
					String toSend = outQueue.Take();
					byte[] bytes = Encoding.ASCII.GetBytes(toSend + (session == null ? "" : "&s=" + session));
					udpClient.Send(bytes, bytes.Length);

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
		public String query(ApiRequest req)
		{
			checkMethodSessionKey(req.Command);

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
			return result;
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

					Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
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

