using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.Hash
{
    enum HashType
    {
        Ed2k,MD4,CRC32,MD5,SHA1
    }
    /// <summary>
    /// HashGenerator
    /// </summary>
    class HashGenerator
    {
        String _sFile;
        System.IO.FileStream _FileStream;
        System.IO.BinaryReader _BinaryReader;
        Int32 _length;
        IHash _hash;
        HashType _htType;
        public HashGenerator(String sFile)
        {
            _sFile = sFile;
            _FileStream = new System.IO.FileStream(sFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            _BinaryReader = new System.IO.BinaryReader(_FileStream);
            _length = (Int32)(new System.IO.FileInfo(sFile).Length);
        }

        public void Generate(HashType htType)
        {
            _htType = htType;
            switch (htType)
            {
                case HashType.Ed2k: _hash = new Ed2k(); break;
                case HashType.MD4: _hash = new MD4(); break;
                case HashType.CRC32: _hash = new Crc32();break ;
                case HashType.MD5: _hash = new MD5(); break;
                case HashType.SHA1: _hash = new Sha1(); break;
                default: throw new NotImplementedException();
            }
            _hash.Reset();
            try
            {
                _hash.Append(_BinaryReader.ReadBytes(_length));
            }
            catch (Exception) { }
            _FileStream.Close();
            _FileStream.Dispose();
            _BinaryReader.Close();
        }
        
        override public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            String sReturn = "";
            byte[] hash = _hash.Finish();
            for (int i = 0; i < hash.Length; i++)
                sb.AppendFormat("{0:x2}", hash[i]);
            switch (_htType)
            {
                case HashType.Ed2k: sReturn = "ed2k://|file|" + _sFile.Substring(_sFile.LastIndexOf(@"\")).Replace(@"\", "") + "|" + _length + "|"+sb.ToString()+"|"; break;
                default: break;
            }
            return sReturn;
        }
    }
}
