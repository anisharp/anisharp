using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API
{
    /// <summary>
    /// Encapsulates an API-Request to AniDB
    /// 
    /// DO NOT set the session with the methods
    /// of this class. It will be set immediately
    /// before sending it.
    /// </summary>
    class ApiRequest
    {
        private Dictionary<String, String> parameters = new Dictionary<string, string>();
        private String command;

        /// <summary>
        /// Creates a command without a command
        /// </summary>
        public ApiRequest()
        {
            command = "NOTGIVEN";
        }

        /// <summary>
        /// Creates a command with the given command and the given parameters
        /// </summary>
        /// <param name="command">like auth or AUTH, automatically uppercased</param>
        /// <param name="data">a param-array of key-value-pairs</param>
        public ApiRequest(String command, params KeyValuePair<String,String>[] data)
        {
            this.Command = command;
            foreach (KeyValuePair<String,String> d in data)
            {
                this[d.Key] = d.Value;
            }
        }
        
        /// <summary>
        /// the command, always uppercased
        /// </summary>
        public String Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value.ToUpper();
            }
        }

        /// <summary>
        /// a parameter
        /// </summary>
        /// <param name="key">the id, like session</param>
        /// <returns>the value</returns>
        public String this[String key]
        {
            get
            {
                return parameters[key];
            }
            set
            {
                parameters[key] = value;
            }
        }

        /// <summary>
        /// sets the specified key and value and returns itself, to allow chaining
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        /// <returns>this</returns>
        public ApiRequest set(String key, String value)
        {
            parameters.Add(key, value);
            return this;
        }

        /// <summary>
        /// converts this Command to a string that can be sent to AniDB
        /// </summary>
        /// <returns>a stringified command</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Command);
            sb.Append(' ');

            foreach (KeyValuePair<String,String> pair in parameters)
            {
                sb.Append(pair.Key).Append('=').Append(pair.Value).Append('&');
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
