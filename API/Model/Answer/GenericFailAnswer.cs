using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    class GenericFailAnswer : ApiAnswer
    {
        public GenericFailAnswer(ReturnCode rc)
            : base(rc)
        {
        }
    }
}
