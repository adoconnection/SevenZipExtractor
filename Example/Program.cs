
using System;
using System.IO;
using SevenZipExtractor;

namespace ConsoleApplication86
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ArchiveFile archiveFile = new ArchiveFile(@"Archive.arj"))
            {
                // extract all
                archiveFile.Extract("Output");
            }

            using (ArchiveFile archiveFile = new ArchiveFile("archive.arj"))
            {
                foreach (Entry entry in archiveFile.Entries)
                {
                    Console.WriteLine(entry.FileName);

                    // extract to file
                    entry.Extract(entry.FileName);

                    // extract to stream
                    MemoryStream memoryStream = new MemoryStream();
                    entry.Extract(memoryStream);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
