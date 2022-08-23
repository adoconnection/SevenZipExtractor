using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class TestArj : TestBase
    {
        [TestMethod]
        public void Text_UnboxAndCast_OK()
        {
            IList<TestFileEntry> testEntries = new List<TestFileEntry>()
            {
                    new TestFileEntry { Name = "AM.EXE", IsFolder = false, CRC32 = "4353180B"},
                    new TestFileEntry { Name = "ANSIMATE.DOC", IsFolder = false, CRC32 = "84DA9118"},
                    new TestFileEntry { Name = "READ_ME!.BAT", IsFolder = false, CRC32 = "882B88D0"},
                    new TestFileEntry { Name = "SEE.COM", IsFolder = false, CRC32 = "88E01284"},
            };

            this.TestExtractToStream(Resources.TestFiles.ansimate_arj, testEntries, SevenZipFormat.Arj);
        }

        [TestMethod]
        public void TestProgressWithArchiveExtraction_OK()
        {
            this.TestExtractArchiveWithProgress(Resources.TestFiles.ansimate_arj, SevenZipFormat.Arj);
        }
        
        [TestMethod]
        public void TestProgressWithEntryExtraction_OK()
        {
            this.TestExtractEntriesWithProgress(Resources.TestFiles.ansimate_arj, SevenZipFormat.Arj);
        }
    }
}