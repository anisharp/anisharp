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
                animeId = x.asInt(1),
                episodeId = x.asInt(2),
                groupId = x.asInt(3),
                state = x.asInt16(4),
                size = x.asInt64(5),
                ed2k = x.asString(6),
                md5 = x.asString(7),
                sha1 = x.asString(8),
                crc32 = x.asString(9),
                quality = x.asString(10),
                source = x.asString(11),
                videoCodec = x.asString(12),
                videoResolution = x.asString(13),
                dubLanguage = x.asString(14),
                subLanguage = x.asString(15),

                epno = x.asString(16),
                epName = x.asString(17),
                epRomajiName = x.asString(18),
                epKanjiName = x.asString(19)
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
