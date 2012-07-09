using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    class MyListEntryAddedAnswer : PipedApiAnswer
    {
        public MyListEntryAddedAnswer(ReturnCode rc, String data)
            : base(rc, data)
        {
        }

        public int MyListId
        {
            get
            {
                return asInt(0);
            }
        }
    }

}
