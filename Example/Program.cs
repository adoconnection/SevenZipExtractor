using System;
using SevenZipExtractor;

namespace ConsoleApplication86
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ArchiveFile archiveFile = new ArchiveFile(@"archive.ARJ"))
            {
                foreach (Entry entry in archiveFile.Entries)
                {
                    Console.WriteLine(entry.FileName);
                    entry.Extract(entry.FileName);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
