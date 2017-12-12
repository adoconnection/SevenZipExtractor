
using System;
using System.IO;
using SevenZipExtractor;

namespace ConsoleApplication86
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Extracting all files from archive.arj");
            using (ArchiveFile archiveFile = new ArchiveFile("archive.arj"))
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

            Console.WriteLine("Extracting 'text.txt' file from 'archive.arj' to C:\\");
            using (ArchiveFile archiveFile = new ArchiveFile("archive.arj"))
            {
                int f = 0;
                while (archiveFile.Entries[f].FileName != "test.txt")
                    if (++f == archiveFile.Entries.Count)
                        Console.WriteLine("test.txt not found in archive.arj");

                archiveFile.Entries[f].Extract(Path.Combine("c:\\", "test.txt"));
            }
            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadKey();


            Console.WriteLine("Extracting 'text.txt' file from 'archive.arj' to memory");
            using (ArchiveFile archiveFile = new ArchiveFile("archive.arj"))
            {
                string txt;

                int f = 0;
                while (archiveFile.Entries[f].FileName != "test.txt")
                    if (++f == archiveFile.Entries.Count)
                        Console.WriteLine("test.txt not found in archive.arj");
                using (MemoryStream _ms = new MemoryStream())
                {
                    archiveFile.Entries[f].Extract(_ms);
                    txt = _ms.ToString();
                }
                Console.WriteLine(txt);
            }
            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadKey();

        }
    }
}
