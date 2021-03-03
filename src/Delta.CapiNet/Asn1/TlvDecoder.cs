using System;
using System.IO;
using System.Collections.Generic;

namespace Delta.CapiNet.Asn1
{
    /// <summary>
    /// Helper class aimed at decoding TLV (Tag, Length, Value)-encoded data.
    /// </summary>
    /// <remarks>
    /// We support tags encoded on up to 2 bytes and length on up to 4 bytes (but stored as int).
    /// </remarks>
    public static class TlvDecoder
    {
        public struct Data
        {
            ////public int TagLength { get; internal set; }

            public byte[] Tag { get; set; }

            public int LengthLength { get; internal set; }

            public int Length { get; set; }

            public byte[] Value { get; set; }
        }
        
        public static Data Decode(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (bytes.Length == 0)
                throw new ArgumentException("No Data", "bytes");

            using (var stream = new MemoryStream(bytes))
            {
                var data = Decode(stream);
                stream.Close();
                return data;
            }
        }

        public static Data Decode(Stream bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            var tag = ReadTag(bytes);
            if (tag == null)
                throw new InvalidDataException("Invalid TLV structure: No Tag.");
            ////var tagLength = rawTag.Length;
            ////var tag = DecodeTag(rawTag);
            
            var rawLength = ReadLength(bytes);
            if (rawLength == null)
                throw new InvalidDataException("Invalid TLV structure: No Length.");
            var lengthLength = rawLength.Length;
            var length = DecodeLength(rawLength);

            var value = ReadValue(bytes, length);

            return new Data()
            {
                Tag = tag,
                ////TagLength = tagLength,
                Length = length,
                LengthLength = lengthLength,
                Value = value
            };
        }

        private static byte[] ReadTag(Stream bytes)
        {
            // See http://en.wikipedia.org/wiki/X.690#Encoding_Structure for TLV parsing
            const byte multiBytesTagMask = 0x1F; // 0x1F: xxx11111
            const byte multiBytesTagLastByteMask = 0x80; // 0x80: 1xxxxxxx
            const int maxTagLength = sizeof(ushort); // 2 bytes

            var eos = false;
            var firstByte = ReadNextByte(bytes, out eos);
            if (eos) 
                return null;

            if ((firstByte & multiBytesTagMask) != multiBytesTagMask) // single-byte tag
                return new byte[] { firstByte };

            // The tag spans multiple bytes
            var list = new List<byte>();
            list.Add(firstByte);

            while (!eos)
            {
                var currentByte = ReadNextByte(bytes, out eos);
                if (eos) continue;

                list.Add(currentByte);

                if ((currentByte & multiBytesTagLastByteMask) != multiBytesTagLastByteMask) // This is the last byte that is part of the tag
                    break;
            }

            if (list.Count == 1)
                throw new InvalidDataException("Invalid TLV structure: expecting more than one byte for this Tag.");

            // maxTagLength + 1 because the first byte will be ignored
            if (list.Count > maxTagLength + 1) throw new InvalidDataException(string.Format(
                "Invalid TLV structure: Tag Length > {0} are not supported.", maxTagLength));

            return list.ToArray();
        }

        private static byte[] ReadLength(Stream bytes)
        {
            const byte multiBytesOrIndefiniteLengthMask = 0x80; // 0x80: 1xxxxxxx
            const byte lengthMask = 0x7F; // 0x7F: x1111111
            const int maxLengthLength = sizeof(ushort); // 2 bytes

            var eos = false;
            var firstByte = ReadNextByte(bytes, out eos);
            if (eos) 
                throw new InvalidDataException("Invalid TLV structure: unexpected End of Stream while determining length.");

            if (firstByte == multiBytesOrIndefiniteLengthMask)
                throw new InvalidDataException("Invalid TLV structure: Indefinite length specification is not supported."); // for now

            if ((firstByte & multiBytesOrIndefiniteLengthMask) != multiBytesOrIndefiniteLengthMask) // single-byte length
                return new byte[] { firstByte };

            var list = new List<byte>();

            list.Add(firstByte);
            // How many bytes should we read to complete the length array?
            var bytesToReadCount = (int)(firstByte & lengthMask);

            if (bytesToReadCount > maxLengthLength) throw new InvalidDataException(string.Format(
                "Invalid TLV structure: Length Length > {0} are not supported.", maxLengthLength));

            for (int i = 0; i < bytesToReadCount; i++)
            {
                var currentByte = ReadNextByte(bytes, out eos);
                if (eos) 
                    throw new InvalidDataException("Invalid TLV structure: unexpected End of Stream while determining length.");

                list.Add(currentByte);
            }
            
            return list.ToArray();
        }

        ////private static ushort DecodeTag(byte[] data)
        ////{
        ////    if (data == null || data.Length == 0)
        ////        throw new InvalidDataException("No Raw Tag data");

        ////    if (data.Length == 1)
        ////        return (ushort)data[0];

        ////    // In multi-byte scenarios, the first byte is a marker and therefore is ignored.
        ////    ushort result = 0;
        ////    for (int i = 1; i < data.Length; i++)
        ////    {
        ////        // Remove bit 8
        ////        var temp = data[i] & 0x7F;

        ////        result += temp;
        ////        result |= data[i];
        ////    }

        ////    return result;
        ////}

        private static int DecodeLength(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new InvalidDataException("No Raw Length data");

            if (data.Length == 1)
                return (int)data[0];

            // In multi-byte scenarios, the first byte is always the multi-byte marker and therefore is ignored.
            var result = 0;
            for (int i = 1; i < data.Length; i++)
            {
                result <<= 8;
                result |= data[i];
            }

            return result;
        }

        private static byte[] ReadValue(Stream bytes, int length)
        {
            if (length == 0)
                return new byte[0];

            var data = new byte[length];
            var readCount = bytes.Read(data, 0, length);
            if (readCount < length) throw new InvalidDataException(string.Format(
                "Invalid TLV structure: unexpected End of Stream while retrieving value: Read bytes count is {0} whereas expected was {1}.",
                readCount, length));

            return data;
        }

        private static byte ReadNextByte(Stream bytes, out bool eos)
        {
            eos = false;
            var current = bytes.ReadByte();
            if (current == -1) // End of stream
                eos = true;

            return (byte)current;
        }
    }
}
