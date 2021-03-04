using System;
using System.Collections.Generic;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet.Asn1
{
    /// <summary>
    /// A tagged object is encoded as a tag, a length and a byte array containing the workload data.
    /// </summary>
    public class TaggedObject
    {
        private static readonly ILogService log = LogManager.GetLogger<TaggedObject>();

        public struct DataTag
        {
            private byte[] tagData;
            private ushort? asn1Tag;

            // The complete Tag; most tags will be one byte long.
            public byte[] FullTagValue
            {
                get => tagData;
                set
                {
                    tagData = value ?? throw new ArgumentNullException("value");
                    asn1Tag = null;
                }
            }

            public ushort Value
            {
                get
                {
                    if (!asn1Tag.HasValue)
                    {
                        if (FullTagValue == null || FullTagValue.Length == 0)
                            return 0xFFFF;

                        if (FullTagValue.Length == 1)
                            asn1Tag = FullTagValue[0];
                        else
                            asn1Tag = (ushort)(FullTagValue[0] << 8 | FullTagValue[1]);
                    }

                    return asn1Tag.Value;
                }
            }

            public bool IsAsn1Tag => FullTagValue.Length == 1;

            public int Length { get; set; }
        }

        public struct DataLength
        {
            public int Value { get; set; }
            public int Length { get; set; }
        }

        protected TaggedObject(byte[] data, int offset, int length)
        {
            EnsureArguments(data, offset, length);
            AllData = data;
            RawDataOffset = offset;
            RawDataLength = length;
        }

        public DataTag Tag { get; private set; }
        public DataLength Length { get;  private set; }

        public byte[] RawData => AllData.SubArray(RawDataOffset, RawDataLength);

        internal byte[] AllData { get; }
        public int RawDataOffset { get; }
        public int RawDataLength { get; }

        public byte[] Workload => AllData.CheckedSubArray(WorkloadOffset, WorkloadLength);
        public int WorkloadOffset => RawDataOffset + WorkloadShift;
        public int WorkloadLength { get; private set; }
        private int WorkloadShift { get; set; }

        internal static TaggedObject[] CreateObjects(byte[] data, int offset, int length)
        {
            try
            {
                EnsureArguments(data, offset, length);

                var taggedObjects = new List<TaggedObject>();
                var buffer = data.SubArray(offset, length);
                var currentOffset = offset;
                var currentLength = length;
                
                while (buffer.Length > 0)
                {
                    var taggedObject = CreateObject(data, currentOffset, currentLength, out var count);
                    if (taggedObject is InvalidTaggedObject)
                        log.Warning("Encountered an invalid tagged object during parsing");

                    if (taggedObject != null)
                    {
                        taggedObjects.Add(taggedObject);
                        currentOffset += count;
                        currentLength -= count;
                        buffer = count >= buffer.Length ? (new byte[0]) : Skip(buffer, count);
                    }
                    else buffer = new byte[0];
                }

                return taggedObjects.ToArray();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return new TaggedObject[0];
        }

        private static TaggedObject CreateObject(byte[] data, int offset, int length, out int count)
        {
            try
            {
                EnsureArguments(data, offset, length);
                var taggedObject = new TaggedObject(data, offset, length);
                count = taggedObject.Parse();
                return taggedObject;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            count = length;
            return new InvalidTaggedObject(data, offset, length);
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns>bytes count of the data used to build the tagged object.</returns>
        private int Parse()
        {
            var result = TlvDecoder.Decode(RawData);
            Tag = new DataTag()
            {
                FullTagValue = result.Tag,
                Length = result.Tag.Length
            };

            WorkloadShift = Tag.Length;
            WorkloadShift += result.LengthLength;
            WorkloadLength = result.Length;

            Length = new DataLength()
            {
                Value = result.Length,
                Length = result.LengthLength
            };

            return Tag.Length + Length.Length + Length.Value;
        }

        private static byte[] Skip(byte[] buffer, int count)
        {
            var length = buffer.Length - count;
            var result = new byte[length];
            Array.Copy(buffer, result, length);
            return result;
        }

        private static void EnsureArguments(byte[] data, int offset, int length)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Length == 0) throw new ArgumentException("Empty data", "data");
            if (offset + length > data.Length) throw new ArgumentOutOfRangeException("data", 
                "input array is not long enough. Check offset and length parameters validity.");
        }
    }
}