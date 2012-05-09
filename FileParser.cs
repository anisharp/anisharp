using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace AniSharp
{
    class FileParser
    {
        private String sFilePattern;
        private String sFile;
        private MainWindow mw;
        public FileParser(MainWindow mw,String sPattern)
        {
            this.mw = mw;
            this.sFilePattern = sPattern;
        }
        public void setFile(String sFile)
        {
            this.sFile = sFile;
        }

        public void ParseFile()
        {
            if(!String.IsNullOrEmpty(sFile))
                ParseFile(sFile);
        }
        private void ParseFile(String sFile, bool isDir = false)
        {
            Regex rg = new Regex(sFilePattern);
            if (!isDir)
            {
                if (!System.IO.Directory.Exists(sFile))
                {
                    if (rg.IsMatch(sFile) && !mw.AnimeCollection.Contains(new AniSharp.MainWindow.Anime(sFile), new AniSharp.MainWindow.AnimeComparer()))
                        mw.lvFiles_Add(sFile);
                }
                else
                    ParseFile(sFile, true);
            }
            else
            {
                try
                {
                    foreach (String s in Directory.GetFiles(sFile))
                    {
                        if (rg.IsMatch(s) && !mw.AnimeCollection.Contains(new AniSharp.MainWindow.Anime(s),new AniSharp.MainWindow.AnimeComparer()))
                            mw.lvFiles_Add(s);
                    }
                }
                catch (Exception) { }
                try
                {
                    foreach (String s in Directory.GetDirectories(sFile))
                        ParseFile(s, true);
                }
                catch (Exception) { }
            }
        }

    }
}
