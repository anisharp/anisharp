using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.Hash
{
    /// <summary>
    /// Made by dmitry.baykov@gmail.com
    /// </summary>
    public class Sha1 : IHash
    {
        static byte[] padding = { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        const UInt32 A = 0x67452301, B = 0xEFCDAB89, C = 0x98BADCFE, D = 0x10325476, E = 0xC3D2E1F0;

        private UInt32 h0 = A, h1 = B, h2 = C, h3 = D, h4 = E;

        private byte[] cache;
        private int cached;
        private UInt64 messageLength;

        public Sha1() { Reset(); }

        public void Reset()
        {
            lock (this)
            {
                h0 = A; h1 = B; h2 = C; h3 = D; h4 = E;
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
            if (buffer == null) throw new ArgumentNullException("buffer");

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
                Append(BigEndian(messageLength));

                byte[] value = new byte[20];
                Array.Copy(BigEndian(h0), 0, value, 0, 4);
                Array.Copy(BigEndian(h1), 0, value, 4, 4);
                Array.Copy(BigEndian(h2), 0, value, 8, 4);
                Array.Copy(BigEndian(h3), 0, value, 12, 4);
                Array.Copy(BigEndian(h4), 0, value, 16, 4);
                return value;
            }
        }

        private void ProcessBlock()
        {
            UInt32 a = h0, b = h1, c = h2, d = h3, e = h4, f, k, t;
            UInt32[] w = new UInt32[80];
            for (int i = 0; i < 16; i++)
                w[i] = BigEndian(cache, i * 4);
            for (int i = 16; i < 80; i++)
                w[i] = leftrotate((w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16]), 1);

            for (int i = 0; i < 80; i++)
            {
                if (i < 20)
                {
                    f = (b & c) | ((~b) & d);
                    k = 0x5A827999;
                }
                else if (i < 40)
                {
                    f = b ^ c ^ d;
                    k = 0x6ED9EBA1;
                }
                else if (i < 60)
                {
                    f = (b & c) | (b & d) | (c & d);
                    k = 0x8F1BBCDC;
                }
                else
                {
                    f = b ^ c ^ d;
                    k = 0xCA62C1D6;
                }
                t = leftrotate(a, 5) + f + e + k + w[i];
                e = d;
                d = c;
                c = leftrotate(b, 30);
                b = a;
                a = t;
            }
            h0 += a; h1 += b; h2 += c; h3 += d; h4 += e;
        }

        static UInt32 leftrotate(UInt32 x, int c) { return (x << c) | (x >> (32 - c)); }

        static byte[] BigEndian(UInt32 value)
        {
            return new byte[] {
				(byte) ((value>>24) & 0xFF),
				(byte) ((value>>16) & 0xFF),
				(byte) ((value>>8) & 0xFF),
				(byte) (value & 0xFF),
			};
        }

        static byte[] BigEndian(UInt64 value)
        {
            return new byte[] {
				(byte) ((value>>56) & 0xFF),
				(byte) ((value>>48) & 0xFF),
				(byte) ((value>>40) & 0xFF),
				(byte) ((value>>32) & 0xFF),
				(byte) ((value>>24) & 0xFF),
				(byte) ((value>>16) & 0xFF),
				(byte) ((value>>8) & 0xFF),
				(byte) (value & 0xFF),
			};
        }

        static UInt32 BigEndian(byte[] value)
        {
            return BigEndian(value, 0);
        }

        static UInt32 BigEndian(byte[] value, int offset)
        {
            return (UInt32)(
                    (value[offset + 0] << 24) |
                    (value[offset + 1] << 16) |
                    (value[offset + 2] << 8) |
                    (value[offset + 3])
                );
        }
    }
}
