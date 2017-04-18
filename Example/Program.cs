using System;
using SevenZipExtractor;
using System.IO;

namespace ConsoleApplication86
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ArchiveFile archiveFile = new ArchiveFile(@"archive.7z"))
            {
                foreach (Entry entry in archiveFile.Entries)
                {
                    if (entry.IsFolder) continue;
                    Console.WriteLine(entry.FileName);
                    Console.WriteLine(string.Format("  Size: {0}", entry.Size));
                    string directory = Path.GetDirectoryName(entry.FileName);
                    if (!string.IsNullOrEmpty(directory)) {
                        Directory.CreateDirectory(directory);
                    }
                    entry.Extract(entry.FileName);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
