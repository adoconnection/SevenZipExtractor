using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenZipExtractor;

namespace SevenZipExtractor.Tests {
    [TestClass]
    public class TestCab : TestBase {
        [TestMethod]
        public void TestGuessAndExtractToStream_OK() {
            this.TestExtractToStream(Resources.TestFiles.cab, this.TestSingleFile);
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK() {
            this.TestExtractToStream(Resources.TestFiles.cab, this.TestSingleFile, SevenZipFormat.Cab);
        }
    }
}
