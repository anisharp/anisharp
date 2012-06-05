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


        // hier werden Locks gesetzt (mit Semaphoren)
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
            Hash.Ed2kHashGenerator hash;
            semHash.WaitOne();
            API.Model.Answer.ApiAnswer answer;
            anime.FileState = "hashing...";
            // hashing done in new block
            {
                hash = hashGen();
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
                if (!checkIfGroupExists(e))
                {
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
                }
                if (!checkIfSerieExists(e))
                {
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
                }
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
            if (mainwin.add != null || mainwin.add != false)
            {
                conn.query(new API.Model.Request.MyListAddRequest(hash.FileSize, hash.Ed2kHash, state: (API.Model.Request.MyListAddRequest.State) mainwin.state ,viewed:mainwin.viewed, viewdate:0, source:mainwin.source, storage:mainwin.storage, other:mainwin.other));
            }
            mainwin.lbLog_Add("Add to Mylist");
        }

        /// <summary>
        /// Generate Ed2k hash from given file
        /// </summary>
        /// <returns>Ed2k hash of given file</returns>
        private Hash.Ed2kHashGenerator hashGen()
        {
            Hash.Ed2kHashGenerator hash = new Hash.Ed2kHashGenerator(anime.FileName);
            hash.waitForIt();
            return hash;
        }

        /// <summary>
        /// Send file request to AniDB
        /// </summary>
        /// <param name="animeId">Id of file to request</param>
        /// <returns>The answer the request recieved</returns>
        private API.Model.Answer.ApiAnswer sendFileRequest(Hash.Ed2kHashGenerator hash)
        {
            API.Model.Request.FileRequest fr = new API.Model.Request.FileRequest(hash.FileSize, hash.Ed2kHash);

            API.Model.Answer.ApiAnswer answer = conn.query(fr);

            return answer;
        }

        /// <summary>
        /// Send group request to AniDB
        /// </summary>
        /// <param name="animeId">id of group to request</param>
        /// <returns>The answer the request recieved</returns>
        private API.Model.Answer.ApiAnswer sendGroupRequest(int groupId)
        {
            API.Model.Request.GroupRequest gr = new API.Model.Request.GroupRequest(groupId);

            API.Model.Answer.ApiAnswer answer = conn.query(gr);

            return answer;
        }

        /// <summary>
        /// Send anime request to AniDB
        /// </summary>
        /// <param name="animeId">Anime id to request</param>
        /// <returns>The answer the request recieved</returns>
        private API.Model.Answer.ApiAnswer sendAnimeRequest(int animeId)
        {
            API.Model.Request.AnimeRequest ar = new API.Model.Request.AnimeRequest(animeId);

            API.Model.Answer.ApiAnswer answer = conn.query(ar);

            return answer;
        }

        /// <summary>
        /// check if group exists in db
        /// </summary>
        /// <param name="e">group to look up</param>
        /// <returns>true if group does not exist in db</returns>
        private bool checkIfGroupExists(episode e)
        {
            groups g = db.getGroup((int)e.groupId);
            return g != null ? true : false;
        }

        /// <summary>
        /// check if anime exists in db
        /// </summary>
        /// <param name="e">anime to look up</param>
        /// <returns>true if anime does not exist in db</returns>
        private bool checkIfSerieExists(episode e)
        {
            serie s = db.getSeries((int)e.animeId);
            return s != null ? true : false;
        }

        /// <summary>
        /// check if episode exists in db
        /// </summary>
        /// <param name="e">episode to look up</param>
        /// <returns>true if episode does not exist in db</returns>
        private bool checkIfEpisodeExists(episode e)
        {
            episode ep = db.getEpisode(e.ed2k);
            return ep != null ? true : false;
        }
    }
}
