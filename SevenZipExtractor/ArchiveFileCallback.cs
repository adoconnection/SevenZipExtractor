using System;
using System.IO;

namespace SevenZipWrapper
{
    internal class ArchiveFileCallback : IArchiveExtractCallback
    {
        private readonly string FileName;
        private readonly uint FileNumber;
        private OutStreamWrapper FileStream;

        public ArchiveFileCallback(uint fileNumber, string fileName)
        {
            this.FileNumber = fileNumber;
            this.FileName = fileName;
        }

        #region IArchiveExtractCallback Members

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
                string FileDir = Path.GetDirectoryName(this.FileName);
                if (!string.IsNullOrEmpty(FileDir))
                {
                    Directory.CreateDirectory(FileDir);
                }
                this.FileStream = new OutStreamWrapper(File.Create(this.FileName));

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
            this.FileStream.Dispose();
            Console.WriteLine(resultEOperationResult);
        }

        #endregion
    }
}