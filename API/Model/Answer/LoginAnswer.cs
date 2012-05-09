using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    /// <summary>
    /// signals successive login
    /// </summary>
    class SuccessfulLoginAnswer : ApiAnswer
    {
        private readonly string sessionkey;

        public SuccessfulLoginAnswer(ReturnCode code, string sessionkey)
            : base(code)
        {
            this.sessionkey = sessionkey;
        }

        /// <summary>
        /// gives the session-key returned by AniDB
        /// </summary>
        public string SessionKey
        {
            get
            {
                return sessionkey;
            }
        }
    }

    /// <summary>
    /// signals failed login
    /// </summary>
    class FailedLoginAnswer : ApiAnswer
    {
        private readonly string message;

        public FailedLoginAnswer(ReturnCode code, string message)
            : base(code)
        {
            this.message = message;
        }

        public String Message
        {
            get
            {
                return message;
            }
        }
    }
}
