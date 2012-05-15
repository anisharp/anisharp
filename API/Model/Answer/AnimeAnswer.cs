using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniSharp;

namespace AniSharp.API.Model.Answer
{
    class AnimeAnswer : PipedApiAnswer
    {
        public AnimeAnswer(ReturnCode code, String data)
            : base(code, data)
        {
        }

        public static implicit operator serie(AnimeAnswer x)
        {
            return new serie
            {
                serienId        = x.asInt(0),
                type            = x.asString(1),
                category        = x.asString(2),
                romajiName      = x.asString(3),
                kanjiName       = x.asString(4),
                englishName     = x.asString(5),
                otherName       = x.asString(6),
                episodes        = x.asInt(7),
                highestNoEp     = x.asInt(8),
                specialEpCount  = x.asInt(9),
                rating          = x.asInt(10),
                tempRating      = x.asInt(11)
            };
        }
    }
}

/*
 < DenisMoskal> jonas: AMask Anime: 
 * 
 * animeID, 
 * type, 
 * category list,  
 * romaji name, 
 * kanji name, 
 * english name, 
 * other name, 
 * episodes, int4
 * highest episode no., int4
 * special ep count, int4
 * rating, int4
 * temp rating int4
*/