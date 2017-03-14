using System.IO;

namespace SevenZipExtractor
{
    public class Entry
    {
        private readonly IInArchive archive;
        private readonly uint index;
        private readonly uint size;
        private readonly uint packedSize;

        internal Entry(IInArchive archive, uint index, uint size, uint packedSize)
        {
            this.archive = archive;
            this.index = index;
            this.size = size;
            this.packedSize = packedSize;
        }

        public string FileName { get; internal set; }
        public bool IsFolder { get; internal set; }
        public uint Size { get; internal set; }
        public uint PackedSize { get; internal set; }

        public void Extract(string fileName)
        {
            using (FileStream fileStream = File.Create(fileName))
            {
                this.Extract(fileStream);
            }
        }
        public void Extract(Stream stream)
        {
            this.archive.Extract(new[] { this.index }, 1, 0, new ArchiveStreamCallback(this.index, stream));
        }
    }
}
