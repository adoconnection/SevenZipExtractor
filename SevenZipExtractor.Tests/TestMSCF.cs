using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenZipExtractor;
using SevenZipExtractor.Tests;
using System.Collections.Generic;
using System.Linq;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestMSCF : TestBase {

        protected IList<TestFileEntry> TestSingleDllFile = new List<TestFileEntry>()
                                         {
                new TestFileEntry { Name = "mslz.dll", IsFolder = false, MD5 = "b3144b66569ab0052b4019a2b4c07a31"},
        };

        [TestMethod]
        public void TestGuessAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.mslz, TestSingleDllFile, null, "mslz.dl_");
        }

        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.mslz, TestSingleDllFile, SevenZipFormat.Mslz, "mslz.dl_");
        }
    }
}