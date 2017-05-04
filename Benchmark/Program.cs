using System;
using SevenZipExtractor;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace ConsoleApplication87 
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
                Console.WriteLine(string.Format(" {0}", entry.FileName));
                Console.WriteLine(string.Format("  Size: {0}", entry.Size));
            }
        }

        // if you want to extract only a few entries from the archive
        // use the Entry.Extract method.
        static void ExtractFirstEntry(ArchiveFile archiveFile) 
        {
            Entry entry = archiveFile.Entries.First(x => !x.IsFolder);
            Console.WriteLine("Extracting first entry, \"" + entry.FileName + "\"");
            entry.Extract(Path.Combine("extracted", Path.GetFileName(entry.FileName)));
        }

        // note that extracting the last entry is a lot slower than extracting
        // the first entry because 7-zip has to decompress the archive stream
        // to seek to the last entry
        static void ExtractLastEntry(ArchiveFile archiveFile) 
        {
            Entry entry = archiveFile.Entries.Last(x => !x.IsFolder);
            Console.WriteLine("Extracting last entry, \"" + entry.FileName + "\"");
            entry.Extract(Path.Combine("extracted", Path.GetFileName(entry.FileName)));
        }

        // if you want to extract all entries from the archive, use ArchiveFile.Extract
        // this is a lot more performant than calling Extract on each entry individually
        static void ExtractAll(string outputPath, ArchiveFile archiveFile) 
        {
            Console.WriteLine("Extracting all entries to \".\\" + outputPath + "\"");
            archiveFile.Extract(outputPath, true);
        }

        // helper function for benchmarking
        static void Benchmark(Action procedure) 
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            procedure();
            stopwatch.Stop();
            Console.WriteLine("Completed in {0} ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("");
        }

        static void Main(string[] args) 
        {
            // provide a path to an archive file here
            string archiveFileName = @"archive.7z";
            string extractTo = Path.Combine("extracted", Path.GetFileNameWithoutExtension(archiveFileName));
            Directory.CreateDirectory("extracted");

            // do some things with the archive
            using (ArchiveFile archiveFile = new ArchiveFile(archiveFileName)) 
            {
                Benchmark(() => PrintEntries(archiveFile));
                Benchmark(() => ExtractFirstEntry(archiveFile));
                Benchmark(() => ExtractLastEntry(archiveFile));
                Benchmark(() => ExtractAll(extractTo, archiveFile));
            }

            // we're done
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
