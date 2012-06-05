using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
namespace AniSharp
{
    class FileRenamer
    {
        private FileRenamer() {}   
        private static volatile FileRenamer instance;
        private DatabaseConnection db = new DatabaseConnection();
        private static object m_lock=new object();
        private MainWindow mw = null;
        private String _Pattern = null;
        private Semaphore _se = new Semaphore(1,1);

        /// <summary>
        /// returns an instance of the FileRenamer class
        /// </summary>
        /// <returns>instance of FileRenamer</returns>
        public static FileRenamer getInstance()
        {
            if(instance==null)
            {
                lock (m_lock)
                {
                    instance = new FileRenamer();
                }
            }
            return instance;
        }

        /// <summary>
        /// set the MainWindow reference
        /// </summary>
        /// <param name="mw">reference of MainWindow</param>
        public void setMainWindow(MainWindow mw)
        {
            if (this.mw == null)
                this.mw = mw;
        }

        /// <summary>
        /// set the renaming pattern for the FileRenamer
        /// </summary>
        /// <param name="sPattern">rename pattern</param>
        public void setPattern(String sPattern)
        {
            if (!String.IsNullOrEmpty(sPattern))
                this._Pattern = sPattern;
        }

        /// <summary>
        /// rename and move the given Anime file
        /// </summary>
        /// <param name="animeFile">Anime file to rename</param>
        public void renameTo(Anime animeFile)
        {
            if (!String.IsNullOrEmpty(_Pattern))
            {
                _se.WaitOne();
                String sPath = animeFile.FileName.Substring(0, animeFile.FileName.LastIndexOf(@"\")+1);
                String sType = animeFile.FileName.Substring(animeFile.FileName.LastIndexOf(@"."));
                String sPattern = _Pattern;
                episode episodes = db.getEpisode(animeFile.FileHash);
                serie series = db.getSeries(episodes.animeId);
                groups group = db.getGroup((int)episodes.groupId) ?? null;
                sPattern = sPattern.Replace("%ann", series.romajiName);
                sPattern = sPattern.Replace("%kan", series.kanjiName);
                String english = (series.englishName != "") ? series.englishName : series.romajiName;
                sPattern = sPattern.Replace("%eng", english);
                sPattern = sPattern.Replace("%epn", episodes.epName);
                sPattern = sPattern.Replace("%epk", episodes.epKanjiName);
                sPattern = sPattern.Replace("%epr", episodes.epRomajiName);
                sPattern = sPattern.Replace("%enr", episodes.epno);
                sPattern = sPattern.Replace("%grp", group.shortName);
                sPattern = sPattern.Replace("%ed2", episodes.ed2k.ToLower());
                sPattern = sPattern.Replace("%ED2", episodes.ed2k.ToUpper());
                sPattern = sPattern.Replace("%md5", episodes.md5.ToLower());
                sPattern = sPattern.Replace("%MD5", episodes.md5.ToUpper());
                sPattern = sPattern.Replace("%sha", episodes.sha1.ToLower());
                sPattern = sPattern.Replace("%SHA", episodes.sha1.ToUpper());
                sPattern = sPattern.Replace("%crc", episodes.crc32.ToLower());
                sPattern = sPattern.Replace("%CRC", episodes.crc32.ToUpper());
                //sName.Replace("%ver",((int)episodes.state)%2);
                //sName.Replace("%cen",
                sPattern = sPattern.Replace("%dub", episodes.dubLanguage);
                sPattern = sPattern.Replace("%sub", episodes.subLanguage);
                sPattern = sPattern.Replace("%vid", episodes.videoCodec);
                sPattern = sPattern.Replace("%qua", episodes.quality);
                sPattern = sPattern.Replace("%src", episodes.source);
                sPattern = sPattern.Replace("%res", episodes.videoResolution);
                sPattern = sPattern.Replace("%eps", series.highestNoEp.ToString());
                sPattern = sPattern.Replace("%typ", series.type);
                sPattern = sPattern.Replace("%gen", series.category);
                sPattern = sPattern.Replace("%aid", episodes.animeId.ToString());
                sPattern = sPattern.Replace("%eid", episodes.episodeId.ToString());
                sPattern = sPattern.Replace(":","");
                sPattern = sPattern.Replace("?", "");
                sPattern = sPattern.Replace("/", "");
                sPattern = sPattern.Replace(@"\", "");
                sPattern = sPattern.Replace("*", "");
                sPattern = sPattern.Replace("<", "");
                sPattern = sPattern.Replace(">", "");
                sPattern = sPattern.Replace("|", "");
                sPattern = sPattern.Replace("\"", "");
                File.Move(animeFile.FileName, sPath + sPattern+sType);
                animeFile.FileName = sPath + sPattern;
                _se.Release();
            }
        }
    }
}
