using System;
using System.Linq;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet
{
    public static class ByteUtils
    {
        private static ILogService log = LogManager.GetLogger(typeof(ByteUtils));

        public static string ToFormattedString(this byte[] array)
        {
            return string.Join(" ", array.Select(b => b.ToString("X2")).ToArray());
        }

        /// <summary>
        /// Returns a copy of the specified array from which the <paramref name="count"/> leading bytes have been removed.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <param name="count">The number of leading bytes to remove.</param>
        /// <returns>Trimmed copy of the byte array.</returns>
        public static byte[] TrimStart(this byte[] data, int count)
        {
            var newLength = data.Length - count;
            var result = new byte[newLength];

            Buffer.BlockCopy(data, count, result, 0, newLength);
            return result;
        }

        /// <summary>
        /// Returns a copy of the specified array from which the <paramref name="count"/> trailing bytes have been removed.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <param name="count">The number of trailing bytes to remove.</param>
        /// <returns>Trimmed copy of the byte array.</returns>
        public static byte[] TrimEnd(this byte[] data, int count)
        {
            var newLength = data.Length - count;
            var result = new byte[newLength];

            Buffer.BlockCopy(data, 0, result, 0, newLength);
            return result;
        }

        /// <summary>
        /// Returns a copy of the specified array from which the <paramref name="leadingCount"/> leading bytes and the
        /// <paramref name="trailingCount"/> bytes have been removed.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <param name="leadingCount">The number of leading bytes to remove.</param>
        /// <param name="trailingCount">The number of trailing bytes to remove.</param>
        /// <returns>Trimmed copy of the byte array.</returns>
        public static byte[] Trim(this byte[] data, int leadingCount, int trailingCount)
        {
            var newLength = data.Length - leadingCount - trailingCount;
            var result = new byte[newLength];

            Buffer.BlockCopy(data, leadingCount, result, 0, newLength);
            return result;
        }

        /// <summary>
        /// Returns a copy of a range of the specified array: starting from <paramref name="offset" />
        /// and copying <paramref name="length" /> bytes.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <param name="offset">The offset from which copy starts in the input array.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <returns>
        /// A copy of the defined range in the input array.
        /// </returns>
        public static byte[] SubArray(this byte[] data, int offset, int length)
        {
            var result = new byte[length];
            Buffer.BlockCopy(data, offset, result, 0, length);
            return result;
        }

        /// <summary>
        /// Returns a copy of a range of the specified array: starting from <paramref name="offset" />
        /// and copying <paramref name="length" /> bytes.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <param name="offset">The offset from which copy starts in the input array.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <returns>
        /// A copy of the defined range in the input array or an empty array if the copy could not succeed.
        /// </returns>
        /// <remarks>
        /// If the parameters are invalid and the copy cannot succeed, an empty array is returned and the error is logged.
        /// </remarks>
        public static byte[] CheckedSubArray(this byte[] data, int offset, int length)
        {
            
            Exception fooException;
            var result = CheckedSubArray(data, offset, length, out fooException);
            if (fooException != null)
                log.Error(fooException);
            return result;
        }

        /// <summary>
        /// Returns a copy of a range of the specified array: starting from <paramref name="offset" />
        /// and copying <paramref name="length" /> bytes.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <param name="offset">The offset from which copy starts in the input array.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <param name="error">Out parameter: an exception that prevented the copy to succeed or <c>null</c>.</param>
        /// <returns>
        /// A copy of the defined range in the input array or an empty array if the copy could not succeed.
        /// </returns>
        /// <remarks>
        /// If the parameters are invalid and the copy cannot succeed, an empty array is returned and the error can be retrieved
        /// in the out <paramref name="error" /> parameter.
        /// </remarks>
        public static byte[] CheckedSubArray(
            this byte[] data, int offset, int length, out Exception error)
        {
            error = null;
            if (data == null)
            {
                error = new ArgumentNullException("data");
                return new byte[0];
            }

            var dataLength = data.Length;
            if (offset < 0)
            {
                error = new ArgumentOutOfRangeException(
                    "offset", "Offset must be strictly greater than 0.");
                return new byte[0];
            }

            if (offset >= dataLength)
            {
                error = new ArgumentOutOfRangeException(
                    "offset", "Offset must be strictly lower than data length.");
                return new byte[0];
            }

            if (length <= 0)
            {
                error = new ArgumentOutOfRangeException(
                    "length", "Length must be greater than 0.");
                return new byte[0];
            }

            if (offset + length > dataLength)
            {
                error = new ArgumentOutOfRangeException(
                    "offset + length", "Offset + Length must be lower than data length.");
                length = dataLength - offset; // let's retrieve what we can.
            }

            return SubArray(data, offset, length);
        }
    }
}
