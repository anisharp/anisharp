using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace AniSharp
{
    class FileRenamer
    {
        private FileRenamer() { }   
        private static volatile FileRenamer instance;
        private DatabaseConnection db = new DatabaseConnection();
        private static object m_lock=new object();
        private MainWindow mw = null;
        private String sPattern = null;
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

        public void setMainWindow(MainWindow mw)
        {
            if (this.mw == null)
                this.mw = mw;
        }

        public void setPattern(String sPattern)
        {
            if (!String.IsNullOrEmpty(sPattern))
                this.sPattern = sPattern;
        }

        public void renameTo(MainWindow.Anime animeFile)
        {
            if (!String.IsNullOrEmpty(sPattern))
            {
                String sPath = animeFile.FileName.Substring(0, animeFile.FileName.LastIndexOf(@"\"));
                String sName = animeFile.FileName.Substring(animeFile.FileName.LastIndexOf(@"\") + 1);
                episode episodes = db.getEpisode(animeFile.FileHash);
                serie series = db.getSeries(episodes.animeId);
                groups group = db.getGroup((int)episodes.groupId) ?? null;
                sName.Replace("%ann",series.romajiName);
                sName.Replace("%kan",series.kanjiName);
                sName.Replace("%eng",series.englishName);
                sName.Replace("%epn",episodes.epName);
                sName.Replace("%epk",episodes.epKanjiName);
                sName.Replace("%epr",episodes.epRomajiName);
                sName.Replace("%enr",episodes.epno);
                sName.Replace("%grp",group.name);
                sName.Replace("%ed2",episodes.ed2k.ToLower());
                sName.Replace("%ED2",episodes.ed2k.ToUpper());
                sName.Replace("%md5",episodes.md5.ToLower());
                sName.Replace("%MD5",episodes.md5.ToUpper());
                sName.Replace("%sha",episodes.sha1.ToLower());
                sName.Replace("%SHA",episodes.sha1.ToUpper());
                sName.Replace("%crc",episodes.crc32.ToLower());
                sName.Replace("%CRC",episodes.crc32.ToUpper());
                //sName.Replace("%ver",((int)episodes.state)%2);
                //sName.Replace("%cen",
                sName.Replace("%dub",episodes.dubLanguage);
                sName.Replace("%sub",episodes.subLanguage);
                sName.Replace("%vid",episodes.videoCodec);
                sName.Replace("%qua",episodes.quality);
                sName.Replace("%src",episodes.source);
                sName.Replace("%res",episodes.videoResolution);
                sName.Replace("%eps",series.highestNoEp.ToString());
                sName.Replace("%typ",series.type);
                sName.Replace("%gen",series.category);
//sName.Replace("%fid",
                sName.Replace("%aid",episodes.animeId.ToString());
                sName.Replace("%eid",episodes.episodeId.ToString());
                sName.Replace("%gid", group.groupsId.ToString());
            }
            /*
            if (sFile != sNew)
            {
                String sPath = sFile.Substring(0, sFile.LastIndexOf(@"\"));
                if (sNew.Contains(sPath))
                    File.Move(sFile, sNew);
                else
                {
                    String sRename = sNew.Substring(sNew.LastIndexOf(@"\"), sNew.Length);
                    File.Move(sFile, sNew);
                }
            }*/
        }
    }
}
