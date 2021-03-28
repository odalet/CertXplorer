using System;
using System.IO;

namespace Delta.CertXplorer.UI
{
    partial class HexViewer
    {
        internal sealed class ByteCollection : System.Collections.CollectionBase
        {
            public ByteCollection() { }
            public ByteCollection(byte[] bs) => AddRange(bs);

            public byte this[int index]
            {
                get => (byte)List[index];
                set => List[index] = value;
            }

            public void Add(byte b) => List.Add(b);
            public void AddRange(byte[] bs) => InnerList.AddRange(bs);
            public void Remove(byte b) => List.Remove(b);
            public void RemoveRange(int index, int count) => InnerList.RemoveRange(index, count);
            public void InsertRange(int index, byte[] bs) => InnerList.InsertRange(index, bs);
            public void Insert(int index, byte b) => InnerList.Insert(index, b);
            public int IndexOf(byte b) => InnerList.IndexOf(b);
            public bool Contains(bool b) => InnerList.Contains(b);
            public void CopyTo(byte[] bs, int index) => InnerList.CopyTo(bs, index);

            public byte[] GetBytes()
            {
                var bytes = new byte[Count];
                InnerList.CopyTo(0, bytes, 0, bytes.Length);
                return bytes;
            }

            public byte[] ToArray()
            {
                var data = new byte[Count];
                CopyTo(data, 0);
                return data;
            }
        }

        internal interface IByteProvider
        {
            event EventHandler LengthChanged;
            event EventHandler Changed;

            long Length { get; }
            bool SupportsWriteByte { get; }
            bool SupportsInsertBytes { get; }
            bool SupportsDeleteBytes { get; }
            bool HasChanges { get; }

            byte ReadByte(long index);
            void WriteByte(long index, byte value);
            void InsertBytes(long index, byte[] bs);
            void DeleteBytes(long index, long length);
            void ApplyChanges();
        }

        internal sealed class ByteArrayProvider : IByteProvider
        {
            public ByteArrayProvider(byte[] content) => Content = content;

            public event EventHandler LengthChanged;
            public event EventHandler Changed;

            public byte[] Content { get; }
            public long Length => Content.Length;
            public bool HasChanges => false;
            public bool SupportsWriteByte => false;
            public bool SupportsInsertBytes => false;
            public bool SupportsDeleteBytes => false;

            public byte ReadByte(long index) => Content[index];
            public void WriteByte(long index, byte value) => throw new NotSupportedException();
            public void InsertBytes(long index, byte[] bs) => throw new NotSupportedException();
            public void DeleteBytes(long index, long length) => throw new NotSupportedException();
            public void ApplyChanges() => throw new NotSupportedException();
        }

        internal sealed class DynamicProvider : IByteProvider
        {
            // Use this provider for a small amount of data

            public DynamicProvider(byte[] data) : this(new ByteCollection(data)) { }
            public DynamicProvider(ByteCollection byteCollection) => Bytes = byteCollection;

            public event EventHandler Changed;
            public event EventHandler LengthChanged;

            public bool HasChanges { get; private set; }

            private void OnChanged(EventArgs e)
            {
                HasChanges = true;
                Changed?.Invoke(this, e);
            }

            private void OnLengthChanged(EventArgs e) => LengthChanged?.Invoke(this, e);

            public ByteCollection Bytes { get; }
            public long Length => Bytes.Count;
            public bool SupportsWriteByte => true;
            public bool SupportsInsertBytes => true;
            public bool SupportsDeleteBytes => true;

            public void ApplyChanges() => HasChanges = false;

            public byte ReadByte(long index) => Bytes[(int)index];

            public void WriteByte(long index, byte value)
            {
                Bytes[(int)index] = value;
                OnChanged(EventArgs.Empty);
            }

            public void DeleteBytes(long index, long length)
            {
                Bytes.RemoveRange(
                    (int)Math.Max(0, index),
                    (int)Math.Min((int)Length, length));

                OnLengthChanged(EventArgs.Empty);
                OnChanged(EventArgs.Empty);
            }

            public void InsertBytes(long index, byte[] bs)
            {
                Bytes.InsertRange((int)index, bs);

                OnLengthChanged(EventArgs.Empty);
                OnChanged(EventArgs.Empty);
            }
        }

        internal sealed class FileProvider : IByteProvider, IDisposable
        {
            private sealed class WriteCollection : System.Collections.DictionaryBase
            {
                public byte this[long index]
                {
                    get => (byte)Dictionary[index];
                    set => Dictionary[index] = value;
                }

                public void Add(long index, byte value) => Dictionary.Add(index, value);
                public bool Contains(long index) => Dictionary.Contains(index);
            }

            public event EventHandler Changed;
            public event EventHandler LengthChanged;

            private readonly WriteCollection bytes = new();
            private FileStream fileStream;

            public FileProvider(string file)
            {
                FileName = file;
                fileStream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }

            public string FileName { get; }
            public bool HasChanges => bytes.Count > 0;
            public long Length => fileStream.Length;
            public bool SupportsWriteByte => true;
            public bool SupportsInsertBytes => false;
            public bool SupportsDeleteBytes => false;

            public void ApplyChanges()
            {
                if (!HasChanges) return;

                var enumerator = bytes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var index = (long)enumerator.Key;
                    var value = (byte)enumerator.Value;
                    if (fileStream.Position != index)
                        fileStream.Position = index;
                    fileStream.Write(new byte[] { value }, 0, 1);
                }
                bytes.Clear();
            }

            public void RejectChanges() => bytes.Clear();

            public byte ReadByte(long index)
            {
                if (bytes.Contains(index))
                    return bytes[index];

                if (fileStream.Position != index)
                    fileStream.Position = index;

                return (byte)fileStream.ReadByte();
            }

            public void WriteByte(long index, byte value)
            {
                if (bytes.Contains(index))
                    bytes[index] = value;
                else bytes.Add(index, value);

                OnChanged(EventArgs.Empty);
            }

            public void DeleteBytes(long index, long length) => throw new NotSupportedException();
            public void InsertBytes(long index, byte[] bs) => throw new NotSupportedException();

            public void Dispose()
            {
                if (fileStream == null) return;
                fileStream.Close();
                fileStream = null;
            }

            private void OnChanged(EventArgs e) => Changed?.Invoke(this, e);
        }
    }
}
