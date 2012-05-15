using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API.Model.Answer
{
    class FileAnswer : PipedApiAnswer
    {
        public FileAnswer(ReturnCode rc, String data)
            : base(rc, data)
        {
        }

        public static implicit operator episode(FileAnswer x)
        {
            return new episode
            {
                animeId = x.asInt(0),
                episodeId = x.asInt(1),
                groupId = x.asInt(2),
                state = x.asInt16(3),
                size = x.asInt64(4),
                ed2k = x.asString(5),
                md5 = x.asString(6),
                sha1 = x.asString(7),
                crc32 = x.asString(8),
                quality = x.asString(9),
                source = x.asString(10),
                videoCodec = x.asString(11),
                videoResolution = x.asString(12),
                dubLanguage = x.asString(13),
                subLanguage = x.asString(14),

                epno = x.asString(15),
                epName = x.asString(16),
                epRomajiName = x.asString(17),
                epKanjiName = x.asString(18)
            };
        }
    }

    /*
17:43 < DenisMoskal> jonas: FMask File: aid, eid, gid, state, size, ed2k, md5, sha1,
                     crc32, quality, source, video codec, video resolution, dub
                     language, sub language
17:46 < DenisMoskal> jonas: AMask File: epno, ep name, ep romaji name, ep kanji
                     name, group name, group short name
17:47 < DenisMoskal> jonas: -group*
*/
}
