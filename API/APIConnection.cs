using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AniSharp.API.Model;

namespace AniSharp.API
{
	class APIConnection
	{
		private String session = null;

		private ApiDriver driver;

		public void establishConnection(String username, String password)
		{
			try
			{
				driver = new ApiDriver();
				driver.connect();

				String loginA = driver.query(new AuthReq(username, password));

				MessageBox.Show(loginA);
				this.session = loginA.Split(' ')[2];
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public void closeUDPClient()
		{
			ApiRequest logoutQ = new ApiRequest("LOGOUT");
			String logoutA = driver.query(logoutQ);
			MessageBox.Show(logoutA);

			driver.disconnect();
		}

		public ApiAnswer query(ApiRequest req)
		{
			// transmit session-key
			if (this.session != null)
				req["s"] = this.session;

			return null;
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
	}
}
