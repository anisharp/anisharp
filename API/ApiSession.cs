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
    /// <summary>
    /// Acts like a regular ApiAdapter, but also handles Sessions
    /// Therefore, username and password is needed to login
    /// </summary>
	class ApiSession
	{
		private String session = null;

		private readonly Queryable decorated;

        /// <summary>
        /// Instantiates a new Session, using the given Queryable
        /// </summary>
        /// <param name="decorated">An instance of a Queryable</param>
        public ApiSession(Queryable decorated)
        {
            this.decorated = decorated;
        }

        /// <summary>
        /// Instantiates a new Session, using the standard implementation
        /// </summary>
        public ApiSession()
            : this(new ApiAdapter())
        {
        }

        /// <summary>
        /// logs the Client in using this Session
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
		public void login(String username, String password)
		{
				ApiAnswer loginA = decorated.query(new AuthRequest(username, password));

				MessageBox.Show(loginA.GetMessage());
				//this.session = loginA.Split(' ')[2];
		}

		public void shutdown()
		{
			ApiAnswer logoutA = decorated.query(new LogoutRequest());
            MessageBox.Show(logoutA.GetMessage());

            decorated.shutdown();
		}

		public ApiAnswer query(ApiRequest req)
		{
			// transmit session-key
            if (this.session != null)
                req["s"] = this.session;
            else
                checkMethodSessionKey(req.Command);

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
