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
        private String _FilePattern;
        private String _File;
        private MainWindow _mw;
        private AniSharp.MainWindow.AnimeComparer ac = new MainWindow.AnimeComparer();
        public FileParser(String File,MainWindow mw,String sPattern)
        {
            this._File = File;
            this._mw = mw;
            this._FilePattern = sPattern;
        }

        public void ParseFile()
        {
            if(!String.IsNullOrEmpty(_File))
                ParseFile(_File);
        }
        private void ParseFile(String sFile, bool isDir = false)
        {
            Regex rg = new Regex(_FilePattern);
            if (!isDir)
            {
                if (!System.IO.Directory.Exists(sFile))
                {
                    lock (_mw.AnimeCollection)
                    {
                        if (rg.IsMatch(sFile))
                            if (!_mw.AnimeCollection.Contains(new AniSharp.MainWindow.Anime(sFile), ac))
                                _mw.lvFiles_Add(sFile);
                    }
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
                        lock (_mw.AnimeCollection)
                        {
                            if (rg.IsMatch(s) && !_mw.AnimeCollection.Contains(new AniSharp.MainWindow.Anime(s), ac))
                                _mw.lvFiles_Add(s);
                        }
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
        /*
        public void ParseFile(Object sFile)
        {
            String sF = (String)sFile;
            Regex rg = new Regex(sFilePattern);

                if (!System.IO.Directory.Exists(sF))
                {
                    if (rg.IsMatch(sF) && !mw.AnimeCollection.Contains(new AniSharp.MainWindow.Anime(sF), ac))
                        mw.lvFiles_Add(sF);
                }
                else
                    ParseFile(sF, true);
        }
        */
    }
}
