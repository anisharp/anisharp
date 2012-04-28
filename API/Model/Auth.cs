
using AniSharp.API;

namespace AniSharp.API.Model {
	
	/// <summary>
	/// Encapsulates a login request
	/// </summary>
	class AuthReq : ApiReq
	{
		/// <summary>
		/// logs a user in by username and password
		/// </summary>
		/// <param name="user">The username at AniDB</param>
		/// <param name="pass">The password</param>
		public Auth(String user, String pass) : ApiReq("AUTH")
		{
			set("user", user);
			set("pass", pass);
			set("protover", AniSharp.Properties.Settings.Default.ApiVersion.ToString());
			set("client", AniSharp.Properties.Settings.Default.ClientName);
			set("clientver", AniSharp.Properties.Settings.Default.Version.ToString());
		}
	}
}

