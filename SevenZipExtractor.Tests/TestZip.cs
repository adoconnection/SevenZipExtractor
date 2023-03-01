using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestZip : TestBase 
    {
        [TestMethod]
        public void TestGuessAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.zip, this.TestEntriesWithFolder);
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.zip, this.TestEntriesWithFolder, SevenZipFormat.Zip);
        }

        [TestMethod]
        public void TestGuessAndExtractToStreamWithFileName_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.zip, this.TestEntriesWithFolder, null, "zip.zip");
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStreamWithFileName_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.zip, this.TestEntriesWithFolder, SevenZipFormat.Zip, "zip.zip");
        }
    }
}