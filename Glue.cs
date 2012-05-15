using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AniSharp
{
    class Glue
    {

        Anime anime;
        API.Application.Queryable conn;

        public Glue(Anime anime, API.Application.Queryable conn)
        {
            this.anime = anime;
            this.conn = conn;
        }

        public void run()
        {
            Hash.Ed2kHashGenerator hash = hashGen();
            anime.FileHash = hash.Ed2kHash;
            API.Model.Answer.ApiAnswer answer = sendToAPI(hash);

            if (answer is API.Model.Answer.FileAnswer)
            {
                API.Model.Answer.FileAnswer fa = (API.Model.Answer.FileAnswer)answer;
                episode e = (episode)fa;
                writeToDB(e);
            }
            else if (answer is API.Model.Answer.GenericFailAnswer)
            {
                //fail message
            }
        }

       private Hash.Ed2kHashGenerator hashGen()
        {
            Hash.Ed2kHashGenerator hash = new Hash.Ed2kHashGenerator(anime.FileName);
            hash.waitForIt();
            return hash;
        }

        private API.Model.Answer.ApiAnswer sendToAPI(Hash.Ed2kHashGenerator hash)
        {
            API.Model.Request.FileRequest fr = new API.Model.Request.FileRequest(hash.FileSize, hash.Ed2kHash);

            API.Model.Answer.ApiAnswer answer = conn.query(fr);

            return answer;
        }

        private void writeToDB(episode e){
            DatabaseConnection db = new DatabaseConnection();
            db.addEntry(e);
        }
    }
}
