using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp
{
    public class AnimeComparer : IEqualityComparer<Anime>
    {

        public bool Equals(Anime a1, Anime a2)
        {
            return a1.FileName == a2.FileName;
        }

        public int GetHashCode(Anime a)
        {
            int hCode = a.FileHash.GetHashCode();
            return hCode.GetHashCode();
        }

    }
}
