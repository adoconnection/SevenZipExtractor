using System;

namespace SevenZipExtractor
{
    public class EntryExtractionProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Number of bytes completed. Can be packed or unpacked size depending on format.
        /// </summary>
        public ulong Completed { get; }

        /// <summary>
        /// The total number of bytes to extract. Not set for some formats.
        /// </summary>
        public ulong Total { get; }

        internal EntryExtractionProgressEventArgs(ulong completed, ulong total)
        {
            Completed = completed;
            Total = total;
        }
    }
}