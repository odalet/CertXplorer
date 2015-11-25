using System;
using System.Linq;
using System.Collections.Generic;

using Delta.CapiNet.Logging;

namespace Delta.CapiNet.Asn1
{
    /// <summary>
    /// A tagged object is encoded as a tag, a length and a byte array containing the workload data.
    /// </summary>
    public class TaggedObject
    {
        private static ILogService log = LogManager.GetLogger<TaggedObject>();

        public struct DataTag
        {
            private byte[] tagData;
            private ushort? asn1Tag;

            // The complete Tag; most tags will be ine byte long.
            public byte[] FullTagValue 
            {
                get { return tagData; }
                set 
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    tagData = value; 
                    asn1Tag = null; 
                }
            }

            public ushort Value
            {
                get
                {
                    if (asn1Tag == null || !asn1Tag.HasValue)
                    {
                        if (FullTagValue == null || FullTagValue.Length == 0)
                            return 0xFFFF; // TODO: Provide a constant to represent invalid tags

                        if (FullTagValue.Length == 1)
                            asn1Tag = (ushort)FullTagValue[0];
                        else
                            asn1Tag = (ushort)(FullTagValue[0] << 8 | FullTagValue[1]);
                    }

                    return asn1Tag.Value;
                }
            }

            public bool IsAsn1Tag
            {
                get { return FullTagValue.Length == 1; }
            }

            public int Length { get; set; }
        }

        public struct DataLength
        {
            public int Value { get; set; }
            public int Length { get; set; }
        }

        private byte[] allData = null;
        private int rawDataOffset = 0;
        private int rawDataLength = 0;
        private int workloadShift = 0;
        private int workloadLength = 0;

        protected TaggedObject(byte[] data, int offset, int length)
        {
            EnsureArguments(data, offset, length);
            allData = data;
            rawDataOffset = offset;
            rawDataLength = length;
        }

        public DataTag Tag
        {
            get; 
            private set;
        }

        public DataLength Length
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets a copy of the raw data.
        /// </summary>
        /// <value>The raw data.</value>
        public byte[] RawData
        {
            get { return allData.SubArray(rawDataOffset, rawDataLength); }
        }

        internal byte[] AllData
        {
            get { return allData; }
        }

        public int RawDataOffset { get { return rawDataOffset; } }

        public int RawDataLength { get { return RawDataLength; } }

        /// <summary>
        /// Gets a copy of the workload data.
        /// </summary>
        /// <value>The workload.</value>
        public byte[] Workload
        {
            get { return allData.CheckedSubArray(WorkloadOffset, WorkloadLength); } 
        }

        public int WorkloadOffset { get { return rawDataOffset + workloadShift; } }

        public int WorkloadLength { get { return workloadLength; } }

        internal static TaggedObject CreateObject(byte[] data, int offset, int length)        
        {
            int count;
            return CreateObject(data, offset, length, out count);
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
                    int count = 0;
                    var taggedObject = CreateObject(data, currentOffset, currentLength, out count);
#if DEBUG
                    if (taggedObject is InvalidTaggedObject)
                    {
#pragma warning disable 219
                        // for debugging purpose: place a breakpoint here
                        var foo = 42;
#pragma warning restore 219
                    }
#endif

                    if (taggedObject != null)
                    {
                        taggedObjects.Add(taggedObject);
                        currentOffset += count;
                        currentLength -= count;

                        if (count >= buffer.Length) 
                            buffer = new byte[0];
                        else buffer = Skip(buffer, count);
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

            workloadShift = Tag.Length;
            workloadShift += result.LengthLength;
            workloadLength = (int)result.Length;

            Length = new DataLength()
            {
                Value = (int)result.Length,
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