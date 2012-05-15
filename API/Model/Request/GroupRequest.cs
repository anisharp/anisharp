using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Request
{
    class GroupRequest : ApiRequest
    {
        public GroupRequest(int gid)
            : base("GROUP")
        {
            set("gid", gid);
        }

        public GroupRequest(string gname)
            : base("GROUP")
        {
            set("gname", gname);
        }

        
    }
}
