using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    /// <summary>
    /// This answer handles all sort of answers with return data seperated by pipes
    /// </summary>
    abstract class PipedApiAnswer : ApiAnswer
    {
        private string[] parts;

        public PipedApiAnswer(ReturnCode code, String data, char seperator = '|') : base(code)
        {
            parts = data.Split(seperator);
        }

        protected String asString(int index)
        {
            return parts[index];
        }

        public int asInt(int index)
        {
            return Int32.Parse(parts[index]);
        }

        protected Int16 asInt16(int index)
        {
            return Int16.Parse(parts[index]);
        }
    }
}
