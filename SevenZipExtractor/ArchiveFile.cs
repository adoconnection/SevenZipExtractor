using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SevenZipExtractor
{
    public class ArchiveFile : IDisposable
    {
        private SevenZipHandle sevenZipHandle;
        private readonly IInArchive archive;
        private readonly InStreamWrapper archiveStream;
        private IList<Entry> entries;

        private string libraryFilePath;

        public ArchiveFile(string archiveFilePath, string libraryFilePath = null)
        {
            this.libraryFilePath = libraryFilePath;

            this.InitializeAndValidateLibrary();

            if (!File.Exists(archiveFilePath))
            {
                throw new SevenZipException("Archive file not found");
            }

            string extension = Path.GetExtension(archiveFilePath);

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new SevenZipException("Unable to guess format for file: " + archiveFilePath);
            }

            string fileExtension = extension.Trim('.').ToLowerInvariant();

            if (!Formats.ExtensionFormatMapping.ContainsKey(fileExtension))
            {
                throw new SevenZipException(fileExtension + " is not a known archive type");
            }

            SevenZipFormat format = this.GuessFormatFromExtension(archiveFilePath, fileExtension);

            this.archive = this.sevenZipHandle.CreateInArchive(Formats.FormatGuidMapping[format]);
            this.archiveStream = new InStreamWrapper(File.OpenRead(archiveFilePath));
        }
        
        public ArchiveFile(Stream archiveStream, SevenZipFormat format, string libraryFilePath = null)
        {
            this.libraryFilePath = libraryFilePath;

            this.InitializeAndValidateLibrary();

            if (archiveStream == null)
            {
                throw new SevenZipException("archiveStream is null");
            }

            this.archive = this.sevenZipHandle.CreateInArchive(Formats.FormatGuidMapping[format]);
            this.archiveStream = new InStreamWrapper(archiveStream);
        }

        public void Extract(string outputFolder, bool overwrite = false) 
        {
            this.Extract(entry => 
            {
                string fileName = Path.Combine(outputFolder, entry.FileName);

                if (entry.IsFolder)
                {
                    return fileName;
                }

                if (!File.Exists(fileName) || overwrite) 
                {
                    return fileName;
                }

                return null;
            });
        }

        public void Extract(Func<Entry, string> getOutputPath) 
        {
            IList<Stream> fileStreams = new List<Stream>();

            try 
            {
                foreach (Entry entry in Entries)
                {
                    string outputPath = getOutputPath(entry);

                    if (outputPath == null) // getOutputPath = null means SKIP
                    {
                        fileStreams.Add(null);
                        continue;
                    }

                    if (entry.IsFolder)
                    {
                        Directory.CreateDirectory(outputPath);
                        fileStreams.Add(null);
                        continue;
                    }

                    string directoryName = Path.GetDirectoryName(outputPath);

                    if (!string.IsNullOrWhiteSpace(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    fileStreams.Add(File.Create(outputPath));
                }

                this.archive.Extract(null, 0xFFFFFFFF, 0, new ArchiveStreamsCallback(fileStreams));
            }
            finally
            {
                foreach (Stream stream in fileStreams) 
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }
            }
        }

        public IList<Entry> Entries
        {
            get
            {
                if (this.entries != null)
                {
                    return this.entries;
                }

                ulong checkPos = 32 * 1024;
                int open = this.archive.Open(this.archiveStream, ref checkPos, null);

                if (open != 0)
                {
                    throw new SevenZipException("Unable to open archive");
                }

                uint itemsCount = this.archive.GetNumberOfItems();

                this.entries = new List<Entry>();

                for (uint fileIndex = 0; fileIndex < itemsCount; fileIndex++)
                {
                    string fileName = this.GetProperty<string>(fileIndex, ItemPropId.kpidPath);
                    bool isFolder = this.GetProperty<bool>(fileIndex, ItemPropId.kpidIsFolder);
                    ulong size = this.GetProperty<ulong>(fileIndex, ItemPropId.kpidSize);
                    ulong packedSize = this.GetProperty<ulong>(fileIndex, ItemPropId.kpidPackedSize);

                    this.entries.Add(new Entry(this.archive, fileIndex)
                    {
                        FileName = fileName,
                        IsFolder = isFolder,
                        Size = size,
                        PackedSize = packedSize
                    });
                }

                return this.entries;
            }
        }

        private T GetProperty<T>(uint fileIndex, ItemPropId name)
        {
            PropVariant propVariant = new PropVariant();
            this.archive.GetProperty(fileIndex, name, ref propVariant);

            T result = propVariant.VarType != VarEnum.VT_EMPTY
                ? (T) propVariant.GetObject()
                : default(T);

            propVariant.Clear();

            return result;
        }

        private void InitializeAndValidateLibrary()
        {
            if (string.IsNullOrWhiteSpace(this.libraryFilePath))
            {
                string suffix = IntPtr.Size == 4 ? "x86" : "x64"; // magic check

                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z-" + suffix + ".dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z-" + suffix + ".dll");
                }
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "7z-" + suffix + ".dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "7z-" + suffix + ".dll");
                }
                else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.dll")))
                {
                    this.libraryFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.dll");
                }
            }

            if (string.IsNullOrWhiteSpace(this.libraryFilePath))
            {
                throw new SevenZipException("libraryFilePath not set");
            }

            if (!File.Exists(this.libraryFilePath))
            {
                throw new SevenZipException("7z.dll not found");
            }

            try
            {
                this.sevenZipHandle = new SevenZipHandle(this.libraryFilePath);
            }
            catch (Exception e)
            {
                throw new SevenZipException("Unable to initialize SevenZipHandle", e);
            }
        }

        private SevenZipFormat GuessFormatFromExtension(string archiveFilePath, string fileExtension)
        {
            if (!fileExtension.Equals("rar", StringComparison.InvariantCultureIgnoreCase))
            {
                return Formats.ExtensionFormatMapping[fileExtension];
            }

            // 7z has different GUID for Pre-RAR5 and RAR5, but they have both same extension (.rar)
            // So check rar file's signature. (https://www.rarlab.com/technote.htm)
            // If it is 0x52 0x61 0x72 0x21 0x1A 0x07 0x01 0x00, then rar file is RAR5.

            byte[] archiveFileSignature = new byte[Formats.RarFiveSignature.Length];

            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            }

            if (Formats.RarFiveSignature.SequenceEqual(archiveFileSignature))
            {
                return SevenZipFormat.Rar5;
            }

            return SevenZipFormat.Rar;
        }

        ~ArchiveFile()
        {
            this.Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (this.archiveStream != null)
            {
                this.archiveStream.Dispose();
            }

            if (this.archive != null)
            {
                Marshal.ReleaseComObject(this.archive);
            }

            if (this.sevenZipHandle != null)
            {
                this.sevenZipHandle.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
