using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestZip : TestBase
    {
        [TestMethod]
        public void TestExtractToStream()
        {
           this.TestExtractToStream(Resources.TestFiles.zip, this.TestEntries);
        }
    }
}