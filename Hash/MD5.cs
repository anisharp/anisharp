using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.Hash
{
    /// <summary>
    /// Made by dmitry.baykov@gmail.com
    /// </summary>
    public class MD5 : IHash
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

        public static UInt32[] T = { 0xD76AA478, 0xE8C7B756, 0x242070DB, 0xC1BDCEEE,
                                      0xF57C0FAF, 0x4787C62A, 0xA8304613, 0xFD469501,
                                      0x698098D8, 0x8B44F7AF, 0xFFFF5BB1, 0x895CD7BE,
                                      0x6B901122, 0xFD987193, 0xA679438E, 0x49B40821,
                                      0xF61E2562, 0xC040B340, 0x265E5A51, 0xE9B6C7AA,
                                      0xD62F105D, 0x02441453, 0xD8A1E681, 0xE7D3FBC8,
                                      0x21E1CDE6, 0xC33707D6, 0xF4D50D87, 0x455A14ED,
                                      0xA9E3E905, 0xFCEFA3F8, 0x676F02D9, 0x8D2A4C8A,
                                      0xFFFA3942, 0x8771F681, 0x6D9D6122, 0xFDE5380C,
                                      0xA4BEEA44, 0x4BDECFA9, 0xF6BB4B60, 0xBEBFBC70,
                                      0x289B7EC6, 0xEAA127FA, 0xD4EF3085, 0x04881D05, 
                                      0xD9D4D039, 0xE6DB99E5, 0x1FA27CF8, 0xC4AC5665,
                                      0xF4292244, 0x432AFF97, 0xAB9423A7, 0xFC93A039, 
                                      0x655B59C3, 0x8F0CCC92, 0xFFEFF47D, 0x85845DD1,
                                      0x6FA87E4F, 0xFE2CE6E0, 0xA3014314, 0x4E0811A1,
                                      0xF7537E82, 0xBD3AF235, 0x2AD7D2BB, 0xeb86d391 };
        static int[] S = { 7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
                            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
                            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
                            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21 };

        private UInt32 h0 = A, h1 = B, h2 = C, h3 = D;

        private byte[] cache;
        private int cached;
        private UInt64 messageLength;

        public MD5() { Reset(); }

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
            UInt32 a = h0, b = h1, c = h2, d = h3, f, g, t;
            UInt32[] w = new UInt32[16];
            for (int i = 0; i < 16; i++)
                w[i] = BitConverter.ToUInt32(cache, i * 4);
            for (UInt32 i = 0; i < 64; i++)
            {
                if (i < 16)
                {
                    f = (b & c) | (~b & d);
                    g = i;
                }
                else if (i < 32)
                {
                    f = (d & b) | (~d & c);
                    g = (5 * i + 1) % 16;
                }
                else if (i < 48)
                {
                    f = b ^ c ^ d;
                    g = (3 * i + 5) % 16;
                }
                else
                {
                    f = c ^ (b | ~d);
                    g = (7 * i) % 16;
                }
                t = d;
                d = c;
                c = b;
                b = b + leftrotate((a + f + T[i] + w[g]), S[i]);
                a = t;
            }
            h0 += a; h1 += b; h2 += c; h3 += d;
        }

        static UInt32 leftrotate(UInt32 x, int c) { return (x << c) | (x >> (32 - c)); }
    }
}
