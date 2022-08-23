﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    [TestClass]
    public class Test7Zip : TestBase
    {
        // 7Z does not provide folder as entry, only files

        [TestMethod]
        public void TestGuessAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.SevenZip, this.TestEntriesWithoutFolder);
        }


        [TestMethod]
        public void TestKnownFormatAndExtractToStream_OK()
        {
            this.TestExtractToStream(Resources.TestFiles.SevenZip, this.TestEntriesWithoutFolder, SevenZipFormat.SevenZip);
        }

        [TestMethod]
        public void TestProgressWithArchiveExtraction_OK()
        {
            this.TestExtractArchiveWithProgress(Resources.TestFiles.SevenZip, SevenZipFormat.SevenZip);
        }
        
        [TestMethod]
        public void TestProgressWithEntryExtraction_OK()
        {
            this.TestExtractEntriesWithProgress(Resources.TestFiles.SevenZip, SevenZipFormat.SevenZip);
        }
    }
}