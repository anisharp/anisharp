using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AniSharp
{
    class Glue
    {
        Anime anime;
        API.Application.Queryable conn;
        DatabaseConnection db;
        private MainWindow mainwin;

        private static Semaphore semHash = new Semaphore(1, 1);
        private static Semaphore semSerie = new Semaphore(1, 1);
        private static Semaphore semGruppe = new Semaphore(1, 1);
        private static Semaphore semApi = new Semaphore(20, 20);
        private static Semaphore semDb = new Semaphore(20, 20);

        public Glue(Anime anime, API.Application.Queryable conn, MainWindow mw)
        {
            this.anime = anime;
            this.conn = conn;
            db = new DatabaseConnection();
            this.mainwin = mw;
        }

        public void run()
        {
            semHash.WaitOne();
            API.Model.Answer.ApiAnswer answer;
            anime.FileState = "hashing...";
            // hashing done in new block
            {
                Hash.Ed2kHashGenerator hash = hashGen();
                mainwin.lbLog_Add("finished hashing " + anime.FileName);

                semHash.Release();
                anime.FileState = "Wait/ID";
                anime.FileHash = hash.Ed2kHash;

                semApi.WaitOne();
                anime.FileState = "identifying...";
                answer = sendFileRequest(hash);
                mainwin.lbLog_Add("querying api with " + hash.Ed2kHash);
                mainwin.lbLog_Add("got answer for " + hash.Ed2kHash + ", it is " + answer.GetType().Name + " with code " + answer.Code);
            }
            System.GC.Collect();
            if (answer is API.Model.Answer.FileAnswer)
            {
                API.Model.Answer.FileAnswer fa = (API.Model.Answer.FileAnswer)answer;
                episode e = (episode)fa;
                semGruppe.WaitOne();
                if (!checkIfGroupExists(e))
                {
                    API.Model.Answer.ApiAnswer ganswer = sendGroupRequest((int)e.groupId);
                    if (ganswer is API.Model.Answer.GroupAnswer)
                    {
                        API.Model.Answer.GroupAnswer ga = (API.Model.Answer.GroupAnswer)ganswer;
                        groups g = (groups)ga;
                        db.addEntry(g);
                        mainwin.lbLog_Add("Group was missing...added");
                    }
                }
                semGruppe.Release();
                semSerie.WaitOne();
                if (!checkIfSerieExists(e))
                {
                    API.Model.Answer.ApiAnswer aanswer = sendAnimeRequest((int)e.animeId);
                    if (aanswer is API.Model.Answer.AnimeAnswer)
                    {
                        API.Model.Answer.AnimeAnswer aa = (API.Model.Answer.AnimeAnswer)aanswer;
                        serie s = (serie)aa;
                        db.addEntry(s);
                        mainwin.lbLog_Add("Serie was missing...added");
                    }
                }
                semSerie.Release();
                anime.FileState = "Wait/Move";
                semApi.Release();
                if (!checkIfEpisodeExists(e))
                {
                    db.addEntry(e);
                    mainwin.lbLog_Add("Episode added");
                }
                else
                {
                    mainwin.lbLog_Add("Episode already in database");
                }
            }
            else if (answer is API.Model.Answer.GenericFailAnswer)
            {
                MessageBox.Show("Server failed.");
            }

            FileRenamer fr = FileRenamer.getInstance();
            mainwin.lbLog_Add("Rename File....move File");
            fr.renameTo(anime);
            anime.FileState = "Finished";
        }

        private Hash.Ed2kHashGenerator hashGen()
        {
            Hash.Ed2kHashGenerator hash = new Hash.Ed2kHashGenerator(anime.FileName);
            hash.waitForIt();
            return hash;
        }

        private API.Model.Answer.ApiAnswer sendFileRequest(Hash.Ed2kHashGenerator hash)
        {
            API.Model.Request.FileRequest fr = new API.Model.Request.FileRequest(hash.FileSize, hash.Ed2kHash);

            API.Model.Answer.ApiAnswer answer = conn.query(fr);

            return answer;
        }

        private API.Model.Answer.ApiAnswer sendGroupRequest(int groupId)
        {
            API.Model.Request.GroupRequest gr = new API.Model.Request.GroupRequest(groupId);

            API.Model.Answer.ApiAnswer answer = conn.query(gr);

            return answer;
        }

        private API.Model.Answer.ApiAnswer sendAnimeRequest(int animeId)
        {
            API.Model.Request.AnimeRequest ar = new API.Model.Request.AnimeRequest(animeId);

            API.Model.Answer.ApiAnswer answer = conn.query(ar);

            return answer;
        }

        private bool checkIfGroupExists(episode e)
        {
            groups g = db.getGroup((int)e.groupId);
            return g != null ? true : false;
        }

        private bool checkIfSerieExists(episode e)
        {
            serie s = db.getSeries((int)e.animeId);
            return s != null ? true : false;
        }
        private bool checkIfEpisodeExists(episode e)
        {
            episode ep = db.getEpisode(e.ed2k);
            return ep != null ? true : false;
        }
    }
}
