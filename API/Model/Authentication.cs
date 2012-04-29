
using System;
using AniSharp.API;

namespace AniSharp.API.Model {
	
	/// <summary>
	/// Encapsulates a login request
	/// </summary>
	class AuthRequest : ApiRequest
	{
		/// <summary>
		/// logs a user in by username and password
		/// </summary>
		/// <param name="user">The username at AniDB</param>
		/// <param name="pass">The password</param>
		public AuthRequest(String user, String pass) : base("AUTH")
		{
			set("user", user);
			set("pass", pass);
			set("protover", AniSharp.Properties.Settings.Default.ApiVersion.ToString());
			set("client", AniSharp.Properties.Settings.Default.ClientName);
			set("clientver", AniSharp.Properties.Settings.Default.Version.ToString());
		}
	}

    /// <summary>
    /// Logs the client out
    /// </summary>
    class LogoutRequest : ApiRequest
    {
        public LogoutRequest() : base("LOGOUT")
        {
        }
    }


    class LoggedInAnswer : ApiAnswer
    {

    }
}

