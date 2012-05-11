using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Request
{
    /// <summary>
    /// Requests information about a file from AniDB
    /// </summary>
    class FileRequest : ApiRequest
    {
        private void setMasks()
        {
            set("fmask", AniSharp.Properties.Settings.Default.fmask_file);
            set("amask", AniSharp.Properties.Settings.Default.amask_file);
        }

        /// <summary>
        /// Search for a file by an fid
        /// </summary>
        /// <param name="id">An fid by AniDB</param>
        public FileRequest(int id)
            : base("FILE")
        {
            set("id", id);
            setMasks();
        }

        /// <summary>
        /// Search for a file by size and Ed2k-Link
        /// </summary>
        /// <param name="size"></param>
        /// <param name="ed2k"></param>
        public FileRequest(Int64 size, String ed2k)
            : base("FILE")
        {
            set("size", size);
            set("ed2k", ed2k);
        }
    }

    /*
     * y fid:

    FILE fid={int4 id}&fmask={hexstr fmask}&amask={hexstr amask} 

by size+ed2k hash:

    FILE size={int8 size}&ed2k={str ed2khash}&fmask={hexstr fmask}&amask={hexstr amask} */
}
