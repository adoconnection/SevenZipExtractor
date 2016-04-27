using System.IO;

namespace SevenZipExtractor
{
    internal class ArchiveStreamCallback : IArchiveExtractCallback
    {
        private readonly uint FileNumber;
        private readonly Stream stream;
        private OutStreamWrapper FileStream;

        public Stream Stream
        {
            get
            {
                return this.stream;
            }
        }

        public ArchiveStreamCallback(uint fileNumber, Stream stream)
        {
            this.FileNumber = fileNumber;
            this.stream = stream;
        }

        public void SetTotal(ulong total)
        {
        }

        public void SetCompleted(ref ulong completeValue)
        {
        }

        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            if ((index == this.FileNumber) && (askExtractMode == AskMode.kExtract))
            {
                this.FileStream = new OutStreamWrapper(this.stream);
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