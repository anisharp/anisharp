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

        public void renameTo(String sFile)
        {
            if (!String.IsNullOrEmpty(sPattern))
            {
                String sPath = sFile.Substring(0, sFile.LastIndexOf(@"\"));
                String sName = sFile.Substring(sFile.LastIndexOf(@"\") + 1);
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
