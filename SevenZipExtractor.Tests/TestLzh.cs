using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestLzh : TestBase
    {
        // LZH does not provide folder as entry, only files

        [TestMethod]
        public void TestGuessAndExtractToStream_Fails()
        {
            Assert.ThrowsException<SevenZipException>(() =>
            {
                this.TestExtractToStream(Resources.TestFiles.lzh, this.TestEntriesWithoutFolder);
            });
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.lzh, this.TestEntriesWithoutFolder, SevenZipFormat.Lzh);
        }
        
        [TestMethod]
        public void TestProgressWithArchiveExtraction_OK()
        {
            this.TestExtractArchiveWithProgress(Resources.TestFiles.lzh, SevenZipFormat.Lzh);
        }
        
        [TestMethod]
        public void TestProgressWithEntryExtraction_OK()
        {
            this.TestExtractEntriesWithProgress(Resources.TestFiles.lzh, SevenZipFormat.Lzh);
        }
    }
}