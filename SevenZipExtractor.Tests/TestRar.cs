using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestRar : TestBase
    {
        [TestMethod]
        public void TestExtractToStream()
        {
            this.TestExtractToStream(Resources.TestFiles.rar, this.TestEntries);
        }
    }
}