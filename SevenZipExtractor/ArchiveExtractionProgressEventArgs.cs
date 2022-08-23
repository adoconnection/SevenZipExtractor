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
        /// The index of the file currently being extracted.
        /// </summary>
        public uint CurrentFileNumber {get; }

        /// <summary>
        /// The total number of files that will be extracted.
        /// </summary>
        public int TotalFileCount {get; }

        internal ArchiveExtractionProgressEventArgs(uint currentFileNumber, int totalFileCount, ulong currentFileCompleted, ulong currentFileTotal) : base(currentFileCompleted, currentFileTotal)
        {
            CurrentFileNumber = currentFileNumber;
            TotalFileCount = totalFileCount;
        }
    }
}
