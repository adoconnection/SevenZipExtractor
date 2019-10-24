namespace SevenZipExtractor.Tests
{
    public struct TestFileEntry
    {
        public string Name { get; set; }
        public bool IsFolder { get; set; }
        public string MD5 { get; set; }
        public string CRC32 { get; set; }
    }
}