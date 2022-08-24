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
        public void TestProgressWithArchiveExtraction_OK()
        {
            this.TestExtractArchiveWithProgress(Resources.TestFiles.zip, SevenZipFormat.Zip);
        }
        
        [TestMethod]
        public void TestProgressWithEntryExtraction_OK()
        {
            this.TestExtractEntriesWithProgress(Resources.TestFiles.zip, SevenZipFormat.Zip);
        }
    }
}