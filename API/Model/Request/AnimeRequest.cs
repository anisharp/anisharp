using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Request
{
    /// <summary>
    /// Encapsulates a request for an anime
    /// </summary>
    class AnimeRequest : ApiRequest
    {
        private string getAmask()
        {
            return AniSharp.Properties.Settings.Default.amask_anime;
        }

        /// <summary>
        /// gets an anime by id
        /// </summary>
        /// <param name="id">the id (as used by AniDB)</param>
        public AnimeRequest(int id)
            : base("ANIME")
        {
            set("aid", id.ToString());
            set("amask", getAmask());
        }

        /// <summary>
        /// gets an anime by name
        /// </summary>
        /// <param name="name">the name used by AniDB</param>
        public AnimeRequest(string name)
            : base("ANIME")
        {
            set("aname", name);
            set("amask", getAmask());
        }
    }
}
