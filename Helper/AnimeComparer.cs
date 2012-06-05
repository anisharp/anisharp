using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp
{
    /// <summary>
    /// AnimeComparer Klasse um zum Vergleich zwischen zwei Identischen FileNames
    /// </summary>
    public class AnimeComparer : IEqualityComparer<Anime>
    {

        /// <summary>
        /// Vergleicht die FileNames der beiden uebergebenen Anime Objekten
        /// </summary>
        /// <param name="a1">Anime Objekt</param>
        /// <param name="a2">Anime Objekt</param>
        /// <returns></returns>
        public bool Equals(Anime a1, Anime a2)
        {
            return a1.FileName == a2.FileName;
        }

        /// <summary>
        /// Gibt einen Hashcode zurueck
        /// </summary>
        /// <param name="a">Anime Objekt</param>
        /// <returns></returns>
        public int GetHashCode(Anime a)
        {
            int hCode = a.FileHash.GetHashCode();
            return hCode.GetHashCode();
        }

    }
}
