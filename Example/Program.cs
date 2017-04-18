using System;
using SevenZipExtractor;
using System.IO;

namespace ConsoleApplication86
{
    class Program
    {
        static void Main(string[] args)
        {
            string archiveFileName = @"archive.7z";
            using (ArchiveFile archiveFile = new ArchiveFile(archiveFileName))
            {
                foreach (Entry entry in archiveFile.Entries)
                {
                    if (entry.IsFolder) continue;
                    Console.WriteLine(entry.FileName);
                    Console.WriteLine(string.Format("  Size: {0}", entry.Size));
                    string outputPath = Path.Combine("extracted", archiveFileName, entry.FileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    entry.Extract(outputPath);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
