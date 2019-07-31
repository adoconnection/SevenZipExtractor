using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestLzh : TestBase
    {
        [TestMethod]
        public void TestGuessAndExtractToStream_Fails()
        {
            Assert.ThrowsException<SevenZipException>(() =>
            {
                this.TestExtractToStream(Resources.TestFiles.lzh, this.TestEntriesWithFolder);
            });
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK()
        {
            // LZH does not provide folder as entry, only files

            this.TestExtractToStream(Resources.TestFiles.lzh, this.TestEntriesWithoutFolder, SevenZipFormat.Lzh);
        }
    }
}