using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    class GroupAnswer : PipedApiAnswer
    {
        //{int gid}|{int4 rating}|{int votes}|{int4 acount}|{int fcount}|
    //{str name}|{str short}|{str irc channel}|{str irc server}|{str url}|{str picname}
    //|{int4 foundeddate}|{int4 disbandeddate}|{int2 dateflags}|{int4 lastreleasedate}|
//{int4 lastactivitydate}|{list grouprelations} 

        public GroupAnswer(ReturnCode rc, String data)
            : base(rc, data)
        {

        }


        public static implicit operator groups(GroupAnswer c)
        {
            return new groups
            {
                groupsId = c.asInt(0),
                rating = c.asInt16(1),
                ircChannel = c.asString(7),
                ircServer = c.asString(8),
                name = c.asString(5),
                shortName = c.asString(6)
            };
        }

    }
}
