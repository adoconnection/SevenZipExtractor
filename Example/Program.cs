using System;
using SevenZipExtractor;
using System.IO;
using System.Linq;

namespace ConsoleApplication86
{
    class Program
    {
        // you can iterate over the entries in an archive and access their properties
        static void PrintEntries(ArchiveFile archiveFile)
        {
            Console.WriteLine("Entries:");
            foreach (Entry entry in archiveFile.Entries)
            {
                if (entry.IsFolder) continue;
                Console.WriteLine(entry.FileName);
                Console.WriteLine(string.Format("  Size: {0}", entry.Size));
            }
            Console.WriteLine("");
        }

        // if you want to extract only a few entries from the archive
        // use the Entry.Extract method.
        static void ExtractFirstEntry(ArchiveFile archiveFile)
        {
            Entry entry = archiveFile.Entries.First(x => !x.IsFolder);
            Console.WriteLine("Extracting " + entry.FileName);
            entry.Extract(Path.GetFileName(entry.FileName));
            Console.WriteLine("");
        }

        // if you want to all entries from the archive, use ArchiveFile.Extract
        static void ExtractAll(string outputPath, ArchiveFile archiveFile) {
            Console.WriteLine("Extracting all entries to \".\\" + outputPath + "\"");
            archiveFile.Extract(outputPath, true);
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            // provide a path to an archive file here
            string archiveFileName = @"archive.7z";

            // do some things with the archive
            using (ArchiveFile archiveFile = new ArchiveFile(archiveFileName))
            {
                PrintEntries(archiveFile);
                ExtractFirstEntry(archiveFile);
                ExtractAll(Path.GetFileNameWithoutExtension(archiveFileName), archiveFile);
            }

            // we're done
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
