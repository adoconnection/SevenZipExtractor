using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SevenZipExtractor
{
    internal class ArchiveStreamsCallback : IArchiveExtractCallback
    {
        private readonly IList<Stream> streams;
        private readonly int streamCount;
        private readonly EventHandler<ArchiveExtractionProgressEventArgs> progressEventHandler;

        private uint currentIndex;
        private ulong currentTotal;
        private ulong currentCompleteValue;
        private bool isCurrentValidForProgress;
        private bool finalProgressReported = false;

        public ArchiveStreamsCallback(IList<Stream> streams, EventHandler<ArchiveExtractionProgressEventArgs> progressEventHandler)
        {
            this.streams = streams;
            this.streamCount = streams.Where(s => s != null).Count();
            this.progressEventHandler = progressEventHandler;
        }

        public void SetTotal(ulong total)
        {
            this.currentTotal = total;
        }

        public void SetCompleted(ref ulong completeValue)
        {
            this.currentCompleteValue = completeValue;

            // If completeValue is 0, currentIndex has not yet been set correctly, since GetStream is initially called after SetCompleted.
            if (completeValue > 0 && this.isCurrentValidForProgress)
            {
                this.InvokeProgressCallback();
            }
        }

        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            this.currentIndex = index;
            this.isCurrentValidForProgress = false;

            if (askExtractMode != AskMode.kExtract)
            {
                outStream = null;
                return 0;
            }

            if (this.streams == null)
            {
                outStream = null;
                return 0;
            }

            Stream stream = this.streams[(int)index];

            if (stream == null)
            {
                outStream = null;
                return 0;
            }

            // SetTotal and SetCompleted are called before GetStream, so now that currentIndex is correct, we invoke the progress callback.
            this.isCurrentValidForProgress = true;
            this.InvokeProgressCallback();

            outStream = new OutStreamWrapper(stream);

            return 0;
        }

        public void PrepareOperation(AskMode askExtractMode)
        {
        }

        public void SetOperationResult(OperationResult resultEOperationResult)
        {
        }
        
        public void InvokeFinalProgressCallback()
        {
            if (!this.finalProgressReported)
            {
                // 7z doesn't invoke SetCompleted for all formats when an entry is fully extracted, so we fake it.
                this.SetCompleted(ref this.currentTotal);
            }
        }

        private void InvokeProgressCallback()
        {
            progressEventHandler?.Invoke(
                this,
                new ArchiveExtractionProgressEventArgs(this.currentIndex, this.streamCount, this.currentCompleteValue, this.currentTotal)
            );

            if (this.currentCompleteValue == this.currentTotal)
            {
                this.finalProgressReported = true;
            }
        }
    }
}