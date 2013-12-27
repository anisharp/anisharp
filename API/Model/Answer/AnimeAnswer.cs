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
                serienId = x.asInt(0),
                type = x.asString(1),
                category = x.asString(2),
                romajiName = x.asString(3),
                kanjiName = x.asString(4),
                englishName = x.asString(5),
                otherName = x.asString(6),
                shortName = x.asString(7),
                episodes = x.asInt(8),
                highestNoEp = x.asInt(9),
                specialEpCount = x.asInt(10),
                rating = x.asInt(11),
                tempRating = x.asInt(12)
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
 * short name,
 * episodes, int4
 * highest episode no., int4
 * special ep count, int4
 * rating, int4
 * temp rating int4
*/