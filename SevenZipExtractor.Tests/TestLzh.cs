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
    }
}