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

        /// <summary>
        /// Computes Adler32 checksum of the specified string.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <returns>The checksum of the bytes resulting from encoding the input string in UTF8.</returns>
        public static long Compute(string data)
        {
            return Compute(data, null);
        }

        /// <summary>
        /// Computes Adler32 checksum of the specified string.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <param name="encoding">The encoding to use to convert the input string to bytes.</param>
        /// <returns>The checksum.</returns>
        public static long Compute(string data, Encoding encoding)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return string.IsNullOrEmpty(data) ?
                Compute((byte[])null) : Compute(encoding.GetBytes(data));
        }

        /// <summary>
        /// Computes the Adler32 checksum of the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The checksum.</returns>
        public static long Compute(byte[] buffer)
        {
            return buffer == null ?
                Compute(buffer, 0, 0) :
                Compute(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Computes the Adler32 checksum of the specified data.
        /// </summary>
        /// <param name="buffer">The buffer containing the data.</param>
        /// <param name="index">The start index of the data in the buffer.</param>
        /// <param name="length">The length of the data in the buffer.</param>
        /// <returns>The checksum.</returns>
        public static long Compute(byte[] buffer, int index, int length)
        {
            // The seed is 1L, equivalent to ComputeCore(0L, null, 0, 0);
            return ComputeCore(1L, buffer, index, length);
        }

        /// <summary>
        /// Computes the Adler32 checksum of the specified data.
        /// </summary>
        /// <param name="adler">The previous data block's Adler32 checksum.</param>
        /// <param name="buffer">The buffer containing the data.</param>
        /// <param name="index">The start index of the data in the buffer.</param>
        /// <param name="length">The length of the data in the buffer.</param>
        /// <returns>The checksum.</returns>
        private static long ComputeCore(long adler, byte[] buffer, int index, int length)
        {
            if (buffer == null) return 1L;

            long left = adler & 0xffff;
            long right = (adler >> 16) & 0xffff;
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
