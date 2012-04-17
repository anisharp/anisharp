using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.Hash
{
    /// <summary>
    /// Made by dmitry.baykov@gmail.com
    /// </summary>
    class Ed2k:IHash
    {
        MD4 md4, root;
        int lastLength;
        const int chunkSize = 9728000;
        bool smallFile = true;

        public Ed2k() { md4 = new MD4(); root = new MD4(); Reset(); }

        public void Reset()
        {
            md4.Reset();
            root.Reset();
            lastLength = 0;
            smallFile = true;
        }

        public void Append(byte value)
        {
            lock (this)
            {
                md4.Append(value);
                lastLength++;
                if (lastLength == chunkSize)
                {
                    smallFile = false;
                    root.Append(md4.Finish());
                    md4.Reset();
                    lastLength = 0;
                }
            }
        }

        public void Append(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException();

            Append(buffer, 0, buffer.Length);
        }
        byte[] cache = new byte[chunkSize];

        public void Append(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (count < 0) throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero");
            if (offset < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException("offset");

            lock (this)
            {
                while (count >= chunkSize - lastLength)
                {
                    Array.Copy(buffer, offset, cache, 0, chunkSize - lastLength);
                    md4.Append(cache, 0, chunkSize - lastLength);
                    root.Append(md4.Finish());
                    md4.Reset();
                    count -= chunkSize - lastLength;
                    offset += chunkSize - lastLength;
                    lastLength = 0;
                    smallFile = false;
                }
                if (count > 0)
                {
                    md4.Append(buffer, offset, count);
                    lastLength += count;
                }
            }
        }

        public byte[] Finish()
        {
            if (smallFile)
                return md4.Finish();
            root.Append(md4.Finish());

            return root.Finish();
        }
    }
}
