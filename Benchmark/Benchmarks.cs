using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using SevenZipExtractor;

namespace Benchmark
{
    [Config(typeof(Config))]
    public class Benchmarks
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(MemoryDiagnoser.Default);
                Add(Job.ShortRun.With(ClrRuntime.Net47));
                Add(Job.ShortRun.With(CoreRuntime.Core31));
            }
        }

        const string ArchiveFileName = @"resources/7z.7z";
        const string Directory = @"extracted";

        private readonly string extractTo = Path.Combine(Directory, Path.GetFileNameWithoutExtension(ArchiveFileName));
        private readonly Consumer consumer = new Consumer();

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.Delete(Directory, true);
            }
            System.IO.Directory.CreateDirectory(Directory);
        }

        // you can iterate over the entries in an archive and access their properties
        [Benchmark]
        public void PrintEntries()
        {
            using (ArchiveFile archiveFile = new ArchiveFile(ArchiveFileName))
            {
                foreach (Entry entry in archiveFile.Entries)
                {
                    if (entry.IsFolder) continue;
                    consumer.Consume(entry.FileName);
                    consumer.Consume(entry.Size);
                }
            }
        }

        // if you want to extract only a few entries from the archive
        // use the Entry.Extract method.
        [Benchmark]
        public void ExtractFirstEntry()
        {
            using (ArchiveFile archiveFile = new ArchiveFile(ArchiveFileName))
            {
                Entry entry = archiveFile.Entries.First(x => !x.IsFolder);
                entry.Extract(Path.Combine(Directory, Path.GetFileName(entry.FileName)));
            }
        }

        // note that extracting the last entry is a lot slower than extracting
        // the first entry because 7-zip has to decompress the archive stream
        // to seek to the last entry
        [Benchmark]
        public void ExtractLastEntry()
        {
            using (ArchiveFile archiveFile = new ArchiveFile(ArchiveFileName))
            {
                Entry entry = archiveFile.Entries.Last(x => !x.IsFolder);
                entry.Extract(Path.Combine(Directory, Path.GetFileName(entry.FileName)));
            }
        }

        // if you want to extract all entries from the archive, use ArchiveFile.Extract
        // this is a lot more performant than calling Extract on each entry individually
        [Benchmark]
        public void ExtractAll()
        {
            using (ArchiveFile archiveFile = new ArchiveFile(ArchiveFileName))
            {
                archiveFile.Extract(extractTo, true);
            }
        }
    }
}