using System.IO;

namespace SevenZipExtractor
{
    public class Entry
    {
        private readonly IInArchive archive;
        private readonly uint index;

        internal Entry(IInArchive archive, uint index)
        {
            this.archive = archive;
            this.index = index;
        }

        public string FileName { get; internal set; }
        public bool IsFolder { get; internal set; }

        public void Extract(string fileName)
        {
            this.Extract(File.Create(fileName));
        }
        public void Extract(Stream stream)
        {
            this.archive.Extract(new[] { this.index }, 1, 0, new ArchiveStreamCallback(this.index, stream));
        }
    }
}