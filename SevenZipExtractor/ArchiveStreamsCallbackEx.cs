using System;
using System.Collections.Generic;
using System.IO;

namespace SevenZipExtractor
{
    internal class ArchiveStreamsCallbackEx : IArchiveExtractCallback
    {
        private readonly IList<Operation> operations;

        public class Operation
        {
            public Entry Entry { get; set; }
            public Func<Entry, Stream> Before { get; set; }
            public Action<OperationResult> After { get; set; }
        }

        public ArchiveStreamsCallbackEx(IList<Operation> operations)
        {
            this.operations = operations;
        }

        public void SetTotal(ulong total)
        {
        }

        public void SetCompleted(ref ulong completeValue)
        {
        }

        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
        {
            if (askExtractMode != AskMode.kExtract)
            {
                outStream = null;
                return 0;
            }

            if (operations == null)
            {
                outStream = null;
                return 0;
            }

            Operation op = operations[(int)index];
            if (op == null)
            {
                outStream = null;
                return 0;
            }
            var stream = op.Before(op.Entry);
            if (stream == null)
            {
                outStream = null;
                return 0;
            }

            currentOperation = op;

            outStream = new OutStreamWrapper(stream);

            return 0;
        }

        public void PrepareOperation(AskMode askExtractMode)
        {
        }

        private Operation currentOperation;

        public void SetOperationResult(OperationResult resultEOperationResult)
        {
            currentOperation?.After(resultEOperationResult);
            currentOperation = null;
        }
    }
}