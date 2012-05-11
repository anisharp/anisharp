using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Request
{
    class FileRequest : ApiRequest
    {
        private void setMasks()
        {
            set("fmask", AniSharp.Properties.Settings.Default.fmask_file);
            set("amask", AniSharp.Properties.Settings.Default.amask_file);
        }

        public FileRequest(int id)
            : base("FILE")
        {
            set("id", id);
            setMasks();
        }

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
