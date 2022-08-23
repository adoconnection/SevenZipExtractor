using System;

namespace SevenZipExtractor
{
    public class EntryExtractionProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Number of bytes completed for file currently being extracted.
        /// </summary>
        public ulong CurrentFileCompleted { get; }

        /// <summary>
        /// The total number of bytes to extract for the current file.
        /// </summary>
        public ulong CurrentFileTotal { get; }

        internal EntryExtractionProgressEventArgs(ulong currentFileCompleted, ulong currentFileTotal)
        {
            CurrentFileCompleted = currentFileCompleted;
            CurrentFileTotal = currentFileTotal;
        }
    }
}