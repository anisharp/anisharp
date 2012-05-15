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
        private String [] _File;
        private MainWindow _mw;
        private AnimeComparer ac = new AnimeComparer();
        public FileParser(String[] File,MainWindow mw,String sPattern)
        {
            this._File = File;
            this._mw = mw;
            this._FilePattern = sPattern;
        }
        public FileParser(String File, MainWindow mw, String sPattern)
        {
            this._File = new String[]{File};
            this._mw = mw;
            this._FilePattern = sPattern;
        }

        public void ParseFile()
        {
           foreach(String s in _File)
            if(!String.IsNullOrEmpty(s))
                ParseFile(s);
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
                            if (!_mw.AnimeCollection.Contains(new Anime(sFile), ac))
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
                            if (rg.IsMatch(s) && !_mw.AnimeCollection.Contains(new Anime(s), ac))
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
    }
}
