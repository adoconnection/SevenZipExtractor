using System;
using System.IO;

namespace SevenZipExtractor
{
    internal class ArchiveStreamCallback : IArchiveExtractCallback
    {
        private readonly uint fileNumber;
        private readonly Stream stream;
        private readonly EventHandler<EntryExtractionProgressEventArgs> progressEventHandler;
        
        private ulong currentCompleteValue;
        private ulong currentTotal;

        public ArchiveStreamCallback(uint fileNumber, Stream stream, EventHandler<EntryExtractionProgressEventArgs> progressEventHandler)
        {
            this.fileNumber = fileNumber;
            this.stream = stream;
            this.progressEventHandler = progressEventHandler;
        }

        public void SetTotal(ulong total)
        {
            this.currentTotal = total;
            InvokeProgressCallback();
        }

        public void SetCompleted(ref ulong completeValue)
        {
            this.currentCompleteValue = completeValue;
            InvokeProgressCallback();
        }

        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            if ((index != this.fileNumber) || (askExtractMode != AskMode.kExtract))
            {
                outStream = null;
                return 0;
            }

            outStream = new OutStreamWrapper(this.stream);

            return 0;
        }

        public void PrepareOperation(AskMode askExtractMode)
        {
        }

        public void SetOperationResult(OperationResult resultEOperationResult)
        {
        }
        
        private void InvokeProgressCallback()
        {
            progressEventHandler?.Invoke(
                this,
                new EntryExtractionProgressEventArgs(this.currentCompleteValue, this.currentTotal)
            );
        }
    }
}