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
        public readonly struct Data
        {
            public Data(byte[] tag, int lengthLength, int length, byte[] value)
            {
                Tag = tag;
                LengthLength = lengthLength;
                Length = length;
                Value = value;
            }

            public byte[] Tag { get; }
            public int LengthLength { get; }
            public int Length { get; }
            public byte[] Value { get; }
        }

        public static Data Decode(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentException("No Data", nameof(bytes));

            using var stream = new MemoryStream(bytes);
            var data = Decode(stream);
            stream.Close();
            return data;
        }

        private static Data Decode(Stream bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var tag = ReadTag(bytes);
            if (tag == null) throw new InvalidDataException("Invalid TLV structure: No Tag.");

            var rawLength = ReadLength(bytes);
            if (rawLength == null) throw new InvalidDataException("Invalid TLV structure: No Length.");

            var lengthLength = rawLength.Length;
            var length = DecodeLength(rawLength);

            var value = ReadValue(bytes, length);

            return new Data(tag, lengthLength, length, value);
        }

        private static byte[] ReadTag(Stream bytes)
        {
            // See http://en.wikipedia.org/wiki/X.690#Encoding_Structure for TLV parsing
            const byte multiBytesTagMask = 0x1F; // 0x1F: xxx11111
            const byte multiBytesTagLastByteMask = 0x80; // 0x80: 1xxxxxxx
            const int maxTagLength = sizeof(ushort); // 2 bytes

            var firstByte = ReadNextByte(bytes, out var endOfStream);
            if (endOfStream) return new byte[0];

            if ((firstByte & multiBytesTagMask) != multiBytesTagMask) // single-byte tag
                return new byte[] { firstByte };

            // The tag spans multiple bytes
            var list = new List<byte> { firstByte };
            while (!endOfStream)
            {
                var currentByte = ReadNextByte(bytes, out endOfStream);
                if (endOfStream) continue;

                list.Add(currentByte);

                // This is the last byte that is part of the tag
                if ((currentByte & multiBytesTagLastByteMask) != multiBytesTagLastByteMask) 
                    break;
            }

            if (list.Count == 1)
                throw new InvalidDataException("Invalid TLV structure: expecting more than one byte for this Tag.");

            // maxTagLength + 1 because the first byte will be ignored
            return list.Count > maxTagLength + 1 ?
                throw new InvalidDataException($"Invalid TLV structure: Tag Length > {maxTagLength} are not supported.") :
                list.ToArray();
        }

        private static byte[] ReadLength(Stream bytes)
        {
            const byte multiBytesOrIndefiniteLengthMask = 0x80; // 0x80: 1xxxxxxx
            const byte lengthMask = 0x7F; // 0x7F: x1111111
            const int maxLengthLength = sizeof(ushort); // 2 bytes

            var firstByte = ReadNextByte(bytes, out var eos);
            if (eos) throw new InvalidDataException("Invalid TLV structure: unexpected End of Stream while determining length.");

            if (firstByte == multiBytesOrIndefiniteLengthMask)
                throw new InvalidDataException("Invalid TLV structure: Indefinite length specification is not supported."); // for now

            if ((firstByte & multiBytesOrIndefiniteLengthMask) != multiBytesOrIndefiniteLengthMask) // single-byte length
                return new byte[] { firstByte };

            var list = new List<byte> { firstByte };
            // How many bytes should we read to complete the length array?
            var bytesToReadCount = firstByte & lengthMask;

            if (bytesToReadCount > maxLengthLength) throw new InvalidDataException(
                $"Invalid TLV structure: Length Length > {maxLengthLength} are not supported.");

            for (var i = 0; i < bytesToReadCount; i++)
            {
                var currentByte = ReadNextByte(bytes, out eos);
                if (eos) 
                    throw new InvalidDataException("Invalid TLV structure: unexpected End of Stream while determining length.");

                list.Add(currentByte);
            }
            
            return list.ToArray();
        }

        private static int DecodeLength(byte[] data)
        {
            if (data == null || data.Length == 0) throw new InvalidDataException("No Raw Length data");
            if (data.Length == 1) return data[0];

            // In multi-byte scenarios, the first byte is always the multi-byte marker and therefore is ignored.
            var result = 0;
            for (var i = 1; i < data.Length; i++)
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
            return readCount < length ? throw new InvalidDataException(
                $"Invalid TLV structure: unexpected End of Stream while retrieving value: Read bytes count is {readCount} whereas expected was {length}.") :
                data;
        }

        private static byte ReadNextByte(Stream bytes, out bool endOfStream)
        {
            endOfStream = false;
            var current = bytes.ReadByte();
            if (current == -1) // End of stream
                endOfStream = true;

            return (byte)current;
        }
    }
}
