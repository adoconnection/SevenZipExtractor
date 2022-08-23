using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZipExtractor.Tests
{
    public abstract class TestBase
    {
        protected IList<TestFileEntry> TestEntriesWithFolder = new List<TestFileEntry>()
        {
                new TestFileEntry { Name = "image1.jpg", IsFolder = false, MD5 = "b3144b66569ab0052b4019a2b4c07a31"},
                new TestFileEntry { Name = "image2.jpg", IsFolder = false, MD5 = "8fdd4013edcf04b335ac3a9ce0c13887"},
                new TestFileEntry { Name = "testFolder", IsFolder = true},
                new TestFileEntry { Name = "testFolder\\image3.jpg", IsFolder = false, MD5 = "24ffd227340432596fe61ef6300098ad"},
        };

        protected IList<TestFileEntry> TestEntriesWithoutFolder = new List<TestFileEntry>()
        {
                new TestFileEntry { Name = "image1.jpg", IsFolder = false, MD5 = "b3144b66569ab0052b4019a2b4c07a31"},
                new TestFileEntry { Name = "image2.jpg", IsFolder = false, MD5 = "8fdd4013edcf04b335ac3a9ce0c13887"},
                new TestFileEntry { Name = "testFolder\\image3.jpg", IsFolder = false, MD5 = "24ffd227340432596fe61ef6300098ad"},
        };

        protected void TestExtractToStream(byte[] archiveBytes, IList<TestFileEntry> expected, SevenZipFormat? sevenZipFormat = null)
        {
            MemoryStream memoryStream = new MemoryStream(archiveBytes);

            using (ArchiveFile archiveFile = new ArchiveFile(memoryStream, sevenZipFormat))
            {
                foreach (TestFileEntry testEntry in expected)
                {
                    Entry entry = archiveFile.Entries.FirstOrDefault(e => e.FileName == testEntry.Name && e.IsFolder == testEntry.IsFolder);

                    Assert.IsNotNull(entry, "Entry not found: " + testEntry.Name);

                    if (testEntry.IsFolder)
                    {
                        continue;
                    }

                    using (MemoryStream entryMemoryStream = new MemoryStream())
                    {
                        entry.Extract(entryMemoryStream);

                        if (testEntry.MD5 != null)
                        {
                            Assert.AreEqual(testEntry.MD5, entryMemoryStream.ToArray().MD5String(), "MD5 does not match: " + entry.FileName);
                        }

                        if (testEntry.CRC32 != null)
                        {
                            Assert.AreEqual(testEntry.CRC32, entryMemoryStream.ToArray().CRC32String(), "CRC32 does not match: " + entry.FileName);
                        }
                    }
                }
            }
        }

        protected void TestExtractEntriesWithProgress(byte[] archiveBytes, SevenZipFormat? sevenZipFormat = null)
        {
            MemoryStream memoryStream = new MemoryStream(archiveBytes);

            using (ArchiveFile archiveFile = new ArchiveFile(memoryStream, sevenZipFormat))
            {
                foreach (var entry in archiveFile.Entries)
                {
                    if (entry.IsFolder)
                    {
                        continue;
                    }

                    using (MemoryStream entryMemoryStream = new MemoryStream())
                    {
                        bool progressCalledAtBeginning = false;
                        bool progressCalledAtEnd = false;

                        entry.Extract(entryMemoryStream, (s, e) =>
                        {
                            if (e.CurrentFileTotal > 0)
                            {
                                if (e.CurrentFileCompleted == 0)
                                {
                                    progressCalledAtBeginning = true;
                                }
                                else if (e.CurrentFileCompleted == e.CurrentFileTotal)
                                {
                                    progressCalledAtEnd = true;
                                }
                            }
                        });

                        Assert.IsTrue(progressCalledAtBeginning, $"Progress callback was not called at the beginning of extracting file {entry.FileName}.");
                        Assert.IsTrue(progressCalledAtEnd, $"Progress callback was not called at the end of extracting file {entry.FileName}.");
                    }
                }
            }
        }

        protected void TestExtractArchiveWithProgress(byte[] archiveBytes, SevenZipFormat? sevenZipFormat = null)
        {
            MemoryStream memoryStream = new MemoryStream(archiveBytes);

            string tempPath = Path.Combine(Path.GetTempPath(), "SevenZipExtractorUnitTests");
            Directory.CreateDirectory(tempPath);

            try
            {
                using (ArchiveFile archiveFile = new ArchiveFile(memoryStream, sevenZipFormat))
                {
                    int progressCalledAtBeginning = 0;
                    int progressCalledAtEnd = 0;
                    HashSet<uint> progressCalledForIndex = new HashSet<uint>();

                    archiveFile.Extract(tempPath, true, (s, e) =>
                    {
                        Assert.AreEqual(archiveFile.Entries.Count, e.TotalFileCount, "Incorrect total file count in progress callback.");
                        progressCalledForIndex.Add(e.CurrentFileNumber);

                        if (e.CurrentFileTotal > 0)
                        {
                            if (e.CurrentFileCompleted == 0)
                            {
                                progressCalledAtBeginning++;
                            }
                            else if (e.CurrentFileCompleted == e.CurrentFileTotal)
                            {
                                progressCalledAtEnd++;
                            }
                        }
                    });

                    Assert.AreEqual(archiveFile.Entries.Count, progressCalledForIndex.Count, "Progress callback was not called at all for one or more files.");
                    Assert.IsTrue(archiveFile.Entries.Count <= progressCalledAtBeginning, "Progress callback was not called at the beginning of extracting each file.");
                    Assert.IsTrue(progressCalledAtEnd > 0, "Progress callback was not called at the end of extracting files.");
                }
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }
    }
}