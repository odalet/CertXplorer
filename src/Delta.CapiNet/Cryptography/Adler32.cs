using System;
using System.Text;

/*
 * This class is based on zlib-1.1.3, so all credit should go authors
 * Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
 * and contributors of zlib.
 */

namespace Delta.CapiNet.Cryptography
{
    // Grabbed from the zlib implementation in BouncyCastle.
    // See http://www.bouncycastle.org/csharp/
    /// <summary>
    /// Implements the Adler32 checksum (see http://en.wikipedia.org/wiki/Adler-32)
    /// </summary>
    public static class Adler32
    {
        // largest prime smaller than 65536
        private const int primeBase = 65521;

        // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
        private const int nmax = 5552;

        public static long Compute(string data) => Compute(data, null);
        public static long Compute(string data, Encoding encoding)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return string.IsNullOrEmpty(data) ?
                Compute((byte[])null) : Compute(encoding.GetBytes(data));
        }

        public static long Compute(byte[] buffer) => Compute(buffer, 0, buffer?.Length ?? 0);
        public static long Compute(byte[] buffer, int index, int length) =>
            // The seed is 1L, equivalent to ComputeCore(0L, null, 0, 0)
            ComputeCore(1L, buffer, index, length);

        private static long ComputeCore(long adler, byte[] buffer, int index, int length)
        {
            if (buffer == null) return 1L;

            var left = adler & 0xffff;
            var right = (adler >> 16) & 0xffff;
            int k;

            while (length > 0)
            {
                k = length < nmax ? length : nmax;
                length -= k;
                while (k >= 16)
                {
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    left += buffer[index++] & 0xff; right += left;
                    k -= 16;
                }

                if (k != 0)
                {
                    do
                    {
                        left += buffer[index++] & 0xff;
                        right += left;
                    }
                    while (--k != 0);
                }

                left %= primeBase;
                right %= primeBase;
            }

            return (right << 16) | left;
        }
    }
}
