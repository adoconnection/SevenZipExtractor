using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenZipExtractor
{
    public class ArchiveExtractionProgressEventArgs : EntryExtractionProgressEventArgs
    {
        /// <summary>
        /// The index of the entry currently being extracted.
        /// </summary>
        public uint EntryIndex {get; }

        /// <summary>
        /// The total number of entries that will be extracted.
        /// </summary>
        public int EntryCount {get; }

        internal ArchiveExtractionProgressEventArgs(uint entryIndex, int entryCount, ulong completed, ulong total) : base(completed, total)
        {
            EntryIndex = entryIndex;
            EntryCount = entryCount;
        }
    }
}
