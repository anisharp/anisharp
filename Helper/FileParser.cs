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

        /// <summary>
        /// Konstruktor fuer den FileParser
        /// </summary>
        /// <param name="File">Liste der Dateien die verarbeitet werden sollen</param>
        /// <param name="mw">Referenz auf das MainWindow</param>
        /// <param name="sPattern">Filter fuer die zu aktzeptierenden Dateiendungen</param>
        public FileParser(String[] File,MainWindow mw,String sPattern)
        {
            this._File = File;
            this._mw = mw;
            this._FilePattern = sPattern;
        }

        /// <summary>
        /// Konstruktor fuer den FileParser
        /// </summary>
        /// <param name="File">Die Dateien die verarbeitet werden sollen</param>
        /// <param name="mw">Referenz auf das MainWindow</param>
        /// <param name="sPattern">Filter fuer die zu aktzeptierenden Dateiendungen</param>
        public FileParser(String File, MainWindow mw, String sPattern)
        {
            this._File = new String[]{File};
            this._mw = mw;
            this._FilePattern = sPattern;
        }

        /// <summary>
        /// Einstiegspunkt fuer den Thread. Verarbeitet die eingetragenen Dateien und uebergibt diese an die ParseFile funktion.
        /// </summary>
        public void ParseFile()
        {
           foreach(String s in _File)
            if(!String.IsNullOrEmpty(s))
                ParseFile(s);
        }

        /// <summary>
        /// Rekursive Funktion um die uebergebenen Dateien (und Dateien innerhab der uebergebenen Ordner) in die AnimeListe einzutragen.
        /// </summary>
        /// <param name="sFile">Datei/Ordner der zur AnimeListe hinzugefuegt werden soll.</param>
        /// <param name="isDir">Optionaler Parameter: falls die uebergebene Datei ein Ordner ist</param>
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
