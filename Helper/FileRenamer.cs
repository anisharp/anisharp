﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using AniSharp.Helper;

namespace AniSharp
{
    /// <summary>
    /// Eine Klasse um ein Anime nach einem bestimmten Muster umzubennen.
    /// Verwendet das Singelton-Pattern
    /// </summary>
    class FileRenamer
    {
        private FileRenamer() {}   
        private static volatile FileRenamer instance;
        private DatabaseConnection db = new DatabaseConnection();
        private static object m_lock=new object();
        private MainWindow mw = null;
        private String _Pattern = null;
        private String _RootPath = null;
        private String _DirectoryPattern = null;
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
        /// set the root directory to where the FileRenamer should move the file
        /// </summary>
        /// <param name="sRootPath">root directory</param>
        public void setRootPath(String sRootPath)
        {
            this._RootPath = sRootPath;
        }

        /// <summary>
        /// set the directory pattern for the FileRenamer
        /// </summary>
        /// <param name="sDirectoryPattern">directory name pattern</param>
        public void setDirectoryPatern(String sDirectoryPattern)
        {
            this._DirectoryPattern = sDirectoryPattern;
        }
        /// <summary>
        /// calls the rename function and moves the given Anime
        /// </summary>
        /// <param name="animeFile">Anime file to rename</param>
        public void renameTo(Anime animeFile)
        {
            _se.WaitOne();
            animeFile.FileState = "moving";
            renameAnime(animeFile);
            _se.Release();
        }

        private void renameAnime(Anime animeFile, bool shorten = false)
        {
            if (!String.IsNullOrEmpty(_Pattern))
            {
                try
                {
                    String sPath = animeFile.FileName.Substring(0, animeFile.FileName.LastIndexOf(@"\") + 1);
                    String sOldPath = sPath;
                    String sType = animeFile.FileName.Substring(animeFile.FileName.LastIndexOf(@"."));
                    String sRenamed = _Pattern;
                    episode episodes = db.getEpisode(animeFile.FileHash);
                    serie series = db.getSeries(episodes.animeId);
                    groups group = db.getGroup((int)episodes.groupId) ?? null;
                    if (!String.IsNullOrEmpty(_RootPath))
                    {
                        sPath = ModifyPathEnding(rename(_RootPath, episodes, series, group, isPath: true, shorten: shorten));
                    }
                    if (!String.IsNullOrEmpty(_DirectoryPattern))
                    {
                        sPath += ModifyPathEnding(rename(_DirectoryPattern, episodes, series, group, shorten: shorten));
                    }
                    sRenamed = rename(_Pattern, episodes, series, group, shorten: shorten) + sType;
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    if (File.Exists(sPath + sRenamed))
                    {
                        mw.lbLog_Add("error while moving file " + animeFile.FileName + " already exists");
                        throw new FileAlreadyExistsException();
                    }
                    File.Move(animeFile.FileName, sPath + sRenamed);
                    animeFile.FileName = sPath + sRenamed;
                    if (IsDirectoryEmpty(sOldPath))
                    {
                        Directory.Delete(sOldPath);
                    }
                }
                catch (PathTooLongException ptle)
                {
                    if (!shorten)
                    {
                        renameAnime(animeFile, true);
                    } 
                    else 
                    {
                        mw.lbLog_Add("error while moving file " + animeFile.FileName + " " + ptle.Message);
                    }
                }
                catch (Exception e)
                {
                      mw.lbLog_Add("error while moving file " + animeFile.FileName + " "+e.Message);
                }
            }
        }

        private static bool IsDirectoryEmpty(String sOldPath)
        {
            return Directory.GetFiles(sOldPath).Length == 0 && Directory.GetDirectories(sOldPath).Length == 0;
        }

        /// <summary>
        /// rename the given string as the pattern says
        /// </summary>
        /// <param name="toRename">String to rename</param>
        /// <param name="episodes">episode object</param>
        /// <param name="series">series object</param>
        /// <param name="group">group object</param>
        /// <param name="isPath">special case if the given string is a path</param>
        /// <returns>the renamed string</returns>
        private String rename(String toRename,episode episodes, serie series, groups group, bool isPath = false,bool shorten=false)
        {
            String english = (series.englishName != "") ? series.englishName : series.romajiName;
            StringBuilder sb = new StringBuilder(toRename)
                .Replace("%ann", (!shorten) ? series.romajiName : series.shortName)
                .Replace("%kan", (!shorten) ? series.kanjiName : series.shortName)
                .Replace("%eng", (!shorten) ? english : series.shortName)
                .Replace("%asn", series.shortName)
                .Replace("%epn", episodes.epName)
                .Replace("%epk", episodes.epKanjiName)
                .Replace("%epr", episodes.epRomajiName)
                .Replace("%enr", episodes.epno)
                .Replace("%grp", group.shortName)
                .Replace("%ed2", episodes.ed2k.ToLower())
                .Replace("%ED2", episodes.ed2k.ToUpper())
                .Replace("%md5", episodes.md5.ToLower())
                .Replace("%MD5", episodes.md5.ToUpper())
                .Replace("%sha", episodes.sha1.ToLower())
                .Replace("%SHA", episodes.sha1.ToUpper())
                .Replace("%crc", episodes.crc32.ToLower())
                .Replace("%CRC", episodes.crc32.ToUpper())
                .Replace("%dub", CleanLanguageString(episodes.dubLanguage))
                .Replace("%sub", CleanLanguageString(episodes.subLanguage))
                .Replace("%vid", episodes.videoCodec)
                .Replace("%qua", episodes.quality)
                .Replace("%src", episodes.source)
                .Replace("%res", episodes.videoResolution)
                .Replace("%eps", series.highestNoEp.ToString())
                .Replace("%typ", series.type)
                .Replace("%gen", series.category)
                .Replace("%aid", episodes.animeId.ToString())
                .Replace("%eid", episodes.episodeId.ToString());
            if (isPath)
            {
                int i=sb.ToString().IndexOf(":")+1;
                sb.Replace(":", "", i, sb.Length - i);
            }
            else
                sb.Replace(":", "");
            sb.Replace("?", "")
                .Replace("/", "")
                .Replace("*", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("\"", "")
                .Replace("´", "")
                .Replace("`", "");
            return sb.ToString();
        }

        /// <summary>
        /// cleans the given language String of duplicates and "unkown" languages
        /// </summary>
        /// <param name="languageString"></param>
        /// <returns></returns>
        private String CleanLanguageString(String languageString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String language in languageString.Split('\''))
            {
                if ("unknown".Equals(language) || sb.ToString().Contains(language))
                {
                    continue;
                }
                sb.Append(language).Append("'");
            }
            sb.Remove(sb.Length - 1, 1); //strip the last '
            return sb.ToString();
        }

        private String ModifyPathEnding(String path){
            if (!path.EndsWith(@"\"))
            {
                return path += @"\";
            }
            return path;
        }
    }
}
