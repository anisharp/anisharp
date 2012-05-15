using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace AniSharp
{
    class Glue
    {
        Anime anime;
        API.Application.Queryable conn;
        private MainWindow mainwin;

        private static Semaphore semHash = new Semaphore(1, 1);
        private static Semaphore semApi = new Semaphore(20, 20);
        private static Semaphore semDb = new Semaphore(20, 20);

        public Glue(Anime anime, API.Application.Queryable conn, MainWindow mw)
        {
            this.anime = anime;
            this.conn = conn;
            this.mainwin = mw;
        }

        public void run()
        {
            semHash.WaitOne();
            Hash.Ed2kHashGenerator hash = hashGen();
            mainwin.lbLog_Add("finished hashing " + anime.FileName);
            semHash.Release();

            anime.FileHash = hash.Ed2kHash;

            semApi.WaitOne();
            mainwin.lbLog_Add("querying api with " + hash.Ed2kHash);
            API.Model.Answer.ApiAnswer answer = sendToAPI(hash);
            mainwin.lbLog_Add("got answer for " + hash.Ed2kHash + ", it is " + answer.GetType().Name + " with code " + answer.Code);
            semApi.Release();

            if (answer is API.Model.Answer.FileAnswer)
            {
                API.Model.Answer.FileAnswer fa = (API.Model.Answer.FileAnswer)answer;
                episode e = (episode)fa;

                semDb.WaitOne();
                writeToDB(e);
                semDb.Release();
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
            //DatabaseConnection db = new DatabaseConnection();
            //db.addEntry(e);
        }
    }
}
