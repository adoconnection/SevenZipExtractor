using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestRar : TestBase
    {
        [TestMethod]
        public void TestGuessAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.rar, this.TestEntriesWithFolder);
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.rar, this.TestEntriesWithFolder, SevenZipFormat.Rar5);
        }        
        
        [TestMethod]
        public void TestProgressWithArchiveExtraction_OK()
        {
            this.TestExtractArchiveWithProgress(Resources.TestFiles.rar, SevenZipFormat.Rar5);
        }
        
        [TestMethod]
        public void TestProgressWithEntryExtraction_OK()
        {
            this.TestExtractEntriesWithProgress(Resources.TestFiles.rar, SevenZipFormat.Rar5);
        }
    }
}