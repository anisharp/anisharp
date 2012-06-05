using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Request
{
    /// <summary>
    /// An Request to add the specified file to your MyList
    /// 
    /// Syntax for the constructors (example):
    /// new MyListAddRequest(100, viewed: true, source: "blubb");
    /// </summary>
    class MyListAddRequest : ApiRequest
    {
        public enum State
        {
            Unknown = 0,
            Hdd = 1,
            Cd = 2,
            Deleted = 3
        }

        /// <summary>
        /// sets additional params
        /// </summary>
        private void setAdditionalParams(State? state = null, bool? viewed = null, Int32? viewdate = null,
            string source = null, string storage = null, string other = null)
        {
            if (state.HasValue)
                set("state", (int) state.Value);

            if (viewed.HasValue)
                set("viewed", (viewed.Value ? "1" : "0"));

            if (viewdate.HasValue)
                set("viewdate", viewdate);

            if (source != null)
                set("source", source);

            if (storage != null)
                set("storage", storage);

            if (other != null)
                set("other", other);
        }
        
        /// <summary>
        /// Creates MyListAdd-Command
        /// </summary>
        /// <param name="fid">The fileid</param>
        /// <param name="state">The state</param>
        /// <param name="viewed">true, if viewed, false, if not</param>
        /// <param name="viewdate">the date of viewing</param>
        /// <param name="source">the source of the file</param>
        /// <param name="storage">where it is stored</param>
        /// <param name="other">some other info</param>
        public MyListAddRequest(int fid,  State? state = null, bool? viewed = null, Int32? viewdate = null,
            string source = null, string storage = null, string other = null)
            : base("MYLISTADD")
        {
            set("fid", fid);

            setAdditionalParams(state, viewed, viewdate, source, storage, other);
        }

        /// <summary>
        /// Creates MyListAdd-Command
        /// </summary>
        /// <param name="size">The size of the file</param>
        /// <param name="ed2khash">The hash of the file</param>
        /// <param name="state">The state</param>
        /// <param name="viewed">true, if viewed, false, if not</param>
        /// <param name="viewdate">the date of viewing</param>
        /// <param name="source">the source of the file</param>
        /// <param name="storage">where it is stored</param>
        /// <param name="other">some other info</param>
        public MyListAddRequest(long size, string ed2khash, State? state = null, bool? viewed = null, Int32? viewdate = null,
            string source = null, string storage = null, string other = null)
            : base("MYLISTADD")
        {
            set("size", size);
            set("ed2k", ed2khash);

            setAdditionalParams(state, viewed, viewdate, source, storage, other);
        }
    }
}
