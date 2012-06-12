
using System;
using AniSharp.API;

namespace AniSharp.API.Model.Request
{

    /// <summary>
    /// Encapsulates a login request
    /// </summary>
    class MessageRequest : ApiRequest
    {
        /// <summary>
        /// logs a user in by username and password
        /// </summary>
        /// <param name="user">The username at AniDB</param>
        /// <param name="pass">The password</param>
        public MessageRequest(String user, String title, String body) : base("SENDMSG")
        {
            set("to", user);
            set("title", title);
            set("body", body);
        }
    }

   
}

