using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    class LogoutAnswer : GenericPositiveAnswer
    {
        public LogoutAnswer(ReturnCode code)
            : base(code)
        {
        }
    }
}
