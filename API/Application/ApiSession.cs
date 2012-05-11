using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AniSharp.API.Model.Request;
using AniSharp.API.Model.Answer;

namespace AniSharp.API.Application
{
    /// <summary>
    /// Acts like a regular ApiAdapter, but also handles Sessions
    /// Therefore, username and password is needed to login
    /// </summary>
	class ApiSession
	{
        /// <summary>
        /// called to notify of changes in the api-session
        /// </summary>
        /// <param name="LoggedIn">true, if logged in, false, if not</param>
        /// <param name="ShouldRetry">if not logged in, signals, whether to retry</param>
        /// <param name="Message">what the frontend should tell the user, if any (may be null)</param>
        public delegate void ApiSessionStatusChangedHandler(bool LoggedIn, bool ShouldRetry, string Message);

        /// <summary>
        /// fired, when the state of this session changes
        /// </summary>
        public event ApiSessionStatusChangedHandler ApiSessionStatusChanged;

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

        // TODO expect 501 and 502 and act accordingly

        /// <summary>
        /// logs the Client in using this Session
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
		public void login(String username, String password)
		{
				ApiAnswer loginAnswer = decorated.query(new AuthRequest(username, password));

                if (loginAnswer != null)
                {
                    if (loginAnswer is SuccessfulLoginAnswer)
                    {
                        SuccessfulLoginAnswer rl = (SuccessfulLoginAnswer)loginAnswer;
                        this.session = rl.SessionKey;

                        if (rl.Code == ReturnCode.LOGIN_ACCEPTED_NEW_VERSION)
                        {
                            //MessageBox.Show("New Version available - please update");
                            ApiSessionStatusChanged(true, false, "New Version available - please update");
                        }
                        else
                        {
                            ApiSessionStatusChanged(true, false, null);
                        }
                    }
                    if (loginAnswer is FailedLoginAnswer)
                    {
                        FailedLoginAnswer fla = (FailedLoginAnswer)loginAnswer;
                        //MessageBox.Show("Login failed -- " + fla.Message);
                        if (fla.Code == ReturnCode.LOGIN_FAILED)
                        {
                            ApiSessionStatusChanged(false, true, "Invalid credentials");
                        }
                        else
                        {
                            ApiSessionStatusChanged(false, false, fla.Message);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("ApiAnswer not parseable");
                }
		}

		public void shutdown()
		{
			ApiAnswer logoutA = query(new LogoutRequest());
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

            return decorated.query(req);
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
