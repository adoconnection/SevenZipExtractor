using System.Collections.Generic;
using System.IO;

namespace SevenZipExtractor
{
    internal class ArchiveStreamsCallback : IArchiveExtractCallback
    {
        private readonly IList<Stream> streams;
        private OutStreamWrapper FileStream;

        public ArchiveStreamsCallback(IList<Stream> streams) 
        {
            this.streams = streams;
        }

        public void SetTotal(ulong total)
        {
        }

        public void SetCompleted(ref ulong completeValue)
        {
        }

        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            if (streams != null) 
            {
                if (streams[(int)index] == null) 
                {
                    outStream = null;
                    return 0;
                }
                this.FileStream = new OutStreamWrapper(streams[(int) index]);
                outStream = this.FileStream;
            }
            else 
            {
                outStream = null;
            }

            return 0;
        }

        public void PrepareOperation(AskMode askExtractMode)
        {
        }

        public void SetOperationResult(OperationResult resultEOperationResult)
        {
        }
    }
}