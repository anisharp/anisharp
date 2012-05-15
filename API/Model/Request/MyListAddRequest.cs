using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Request
{
    class MyListAddRequest : ApiRequest
    {
        public MyListAddRequest(int fid)
            : base("MYLISTADD")
        {
            set("fid", fid);
        }

        public MyListAddRequest(long size, string ed2khash)
            : base("MYLISTADD")
        {
            set("size", size);
            set("ed2k", ed2khash);
        }
    }
}
