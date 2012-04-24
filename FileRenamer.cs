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

        public void RenameTo(String sFile,String sNew)
        {
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
            }
        }
    }
}
