﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
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


        protected ApiAnswer(ReturnCode code)
        {
            this.code = code;
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
                    return new SuccessfulLoginAnswer(rc, parts[1]);
                case ReturnCode.LOGIN_FAILED:
                case ReturnCode.CLIENT_VERSION_OUTDATED:
                case ReturnCode.BANNED:
                case ReturnCode.ILLEGAL_INPUT_OR_ACCESS_DENIED:
                case ReturnCode.ANIDB_OUT_OF_SERVICE:
                    return new FailedLoginAnswer(rc, rc.ToString());

                case ReturnCode.LOGGED_OUT:
                    return new LogoutAnswer(rc);
                case ReturnCode.NOT_LOGGED_IN:
                    return new GenericFailAnswer(rc);



            }

            return null;
        }
    }
}

