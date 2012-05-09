﻿
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using AniSharp.API.Transport;
using AniSharp.API.Model.Answer;

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
		public const string TAG_NOT_GIVEN_TAG = "";



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
                    lock (toSend)
                    {
                        System.Diagnostics.Debug.Print("got lock for command, sending now");
                        udpadapter.send(toSend);

                        Monitor.PulseAll(toSend);
                    }

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

			if (req.Command.Equals("AUTH"))
			{
				req["enc"] = AniSharp.Properties.Settings.Default.TransportEncoding;
                System.Diagnostics.Debug.Print("This is a login, so appending enc");
			}
            String outString = req.ToString();
            lock (outString)
            {
                outQueue.Add(outString);

                if (req.Command.Equals("AUTH"))
                {
                    System.Diagnostics.Debug.Print("waiting for outString to be pulsed");
                    Monitor.Wait(outString);
                    System.Diagnostics.Debug.Print("outString pulsed, setting encoding");
                    udpadapter.TransportEncoding = Encoding.GetEncoding(AniSharp.Properties.Settings.Default.TransportEncoding);
                }
            }
            System.Diagnostics.Debug.Print("command enqueued, waiting for result");
			while (!results.ContainsKey(tag))
				lock (results)
				{
					Monitor.Wait(results);
				}
            System.Diagnostics.Debug.Print("and we have result");
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

					int index = returnData.IndexOf(' ');
					if (index < 0)
					{
						// we received garbage
						System.Diagnostics.Debug.Print("received garbage from AniDB: " + returnData + '\n');
						continue;
					}

					String firstElem = returnData.Substring(0, index);
					if (firstElem.Length == 3 && Int32.Parse(firstElem) > 0)
					{
						// not tagged
						results.Add(TAG_NOT_GIVEN_TAG, returnData);
					}
					else
					{
						// we have tag
						String strippedData = returnData.Substring(index + 1);
						results.Add(firstElem, strippedData);
					}

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

