using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.Hash
{
    /// <summary>
    /// Made by dmitry.baykov@gmail.com
    /// </summary>
    interface IHash
    {
        void Reset();
        void Append(byte value);
        void Append(byte[] buffer);
        void Append(byte[] buffer, int offset, int count);
        byte[] Finish();
    }
}
