using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.Hash
{
    /// <summary>
    /// Made by dmitry.baykov@gmail.com
    /// </summary>
    class MD4
    {
        static byte[] padding = { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        const UInt32 A = 0x67452301, B = 0xEFCDAB89, C = 0x98BADCFE, D = 0x10325476;

        static int[] O = {
                              0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
                              0,  4,  8, 12,  1,  5,  9, 13,  2,  6, 10, 14,  3,  7, 11, 15,
                              0,  8,  4, 12,  2, 10,  6, 14,  1,  9,  5, 13,  3, 11,  7, 15
                          };

        static int[] S = {
                              3,  7, 11, 19,  3,  7, 11, 19,  3,  7, 11, 19,  3,  7, 11, 19,
                              3,  5,  9, 13,  3,  5,  9, 13,  3,  5,  9, 13,  3,  5,  9, 13,
                              3,  9, 11, 15,  3,  9, 11, 15,  3,  9, 11, 15,  3,  9, 11, 15
                          };

        private byte[] cache;
        private int cached;
        private UInt64 messageLength;

        private UInt32 h0 = A, h1 = B, h2 = C, h3 = D;

        public MD4() { Reset(); }

        public void Reset()
        {
            lock (this)
            {
                h0 = A; h1 = B; h2 = C; h3 = D;
                cache = new byte[64];
                cached = 0;
                messageLength = 0;
            }
        }

        public void Append(byte value)
        {
            lock (this)
            {
                messageLength++;
                if (cached == 63)
                {
                    cache[63] = value;
                    ProcessBlock();
                    cached = 0;
                }
                else
                {
                    cache[cached] = value;
                    cached++;
                }
            }
        }

        public void Append(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            Append(buffer, 0, buffer.Length);
        }

        public void Append(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (count < 0) throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero");
            if (offset < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException("offset");

            lock (this)
            {
                messageLength += (uint)count;
                while (count >= 64 - cached)
                {
                    Array.Copy(buffer, offset, cache, cached, 64 - cached);
                    ProcessBlock();
                    count -= 64 - cached;
                    offset += 64 - cached;
                    cached = 0;
                }
                if (count > 0)
                {
                    Array.Copy(buffer, offset, cache, cached, count);
                    cached += count;
                }
            }
        }

        public byte[] Finish()
        {
            lock (this)
            {
                int len = 56 - cached;
                if (len <= 0) len += 64;
                UInt64 messageLength = this.messageLength * 8;
                Append(padding, 0, len);
                Append(BitConverter.GetBytes(messageLength));

                byte[] value = new byte[16];
                Array.Copy(BitConverter.GetBytes(h0), 0, value, 0, 4);
                Array.Copy(BitConverter.GetBytes(h1), 0, value, 4, 4);
                Array.Copy(BitConverter.GetBytes(h2), 0, value, 8, 4);
                Array.Copy(BitConverter.GetBytes(h3), 0, value, 12, 4);
                return value;
            }
        }

        private void ProcessBlock()
        {
            UInt32 a = h0, b = h1, c = h2, d = h3, f, t, p;
            UInt32[] w = new UInt32[16];
            for (int i = 0; i < 16; i++)
                w[i] = BitConverter.ToUInt32(cache, i * 4);
            for (UInt32 i = 0; i < 48; i++)
            {
                if (i < 16)
                {
                    f = (b & c) | ((~b) & d);
                    p = 0;
                }
                else if (i < 32)
                {
                    f = (b & c) | (b & d) | (c & d);
                    p = 0x5A827999;
                }
                else
                {
                    f = b ^ c ^ d;
                    p = 0x6ED9EBA1;
                }

                t = d;
                d = c;
                c = b;
                b = leftrotate(a + f + w[O[i]] + p, S[i]);
                a = t;
            }
            h0 += a; h1 += b; h2 += c; h3 += d;
        }

        static UInt32 leftrotate(UInt32 x, int c) 
        { 
            return (x << c) | (x >> (32 - c)); 
        }

    }
}
