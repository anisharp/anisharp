using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API
{

    /// <summary>
    /// Encapsulats an API-Answer
    /// </summary>
    public class ApiAnswer
    {

        private ReturnCode code;
        public ReturnCode Code
        {
            get
            {
                return code;
            }
        }


        public String GetMessage()
        {
            return code.ToString("F");
        }


        protected ApiAnswer(int code)
        {
            try
            {
                this.code = (ReturnCode)code;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }
        }


        public static ApiAnswer parse(String s)
        {
            String[] parts = s.Split(' ');
            int code = Int32.Parse(parts[0]);
            ReturnCode rc;
            try
            {
                rc = (ReturnCode)code;
            }
            catch (InvalidCastException)
            {
                // unknown return code
                throw new ArgumentException("Unknown ReturnCode");
            }

            switch (rc)
            {
                case ReturnCode.LOGIN_ACCEPTED:
                case ReturnCode.LOGIN_ACCEPTED_NEW_VERSION:

                    break;


            }

            return null;
        }
    }
}
