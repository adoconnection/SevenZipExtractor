using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZipExtractor
{
    public class ArchiveFile : IDisposable
    {
        private SevenZipFormat sevenZipFormat;
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

            KnownSevenZipFormat format;
            string fileExtension = Path.GetExtension(archiveFilePath).Trim('.');

            if (!Enum.TryParse(fileExtension, true, out format))
            {
                throw new SevenZipException(fileExtension + " is not a known archive type");
            }

            
            this.archive = this.sevenZipFormat.CreateInArchive(SevenZipFormat.GetClassIdFromKnownFormat(format));
            this.archiveStream = new InStreamWrapper(File.OpenRead(archiveFilePath));
        }

        public ArchiveFile(Stream archiveStream, KnownSevenZipFormat format, string libraryFilePath = null)
        {
            this.libraryFilePath = libraryFilePath;

            this.InitializeAndValidateLibrary();

            if (archiveStream == null)
            {
                throw new SevenZipException("archiveStream is null");
            }

            this.archive = this.sevenZipFormat.CreateInArchive(SevenZipFormat.GetClassIdFromKnownFormat(format));
            this.archiveStream = new InStreamWrapper(archiveStream);
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

                uint fileIndex = 0;

                for (; fileIndex < itemsCount; fileIndex++)
                {
                    PropVariant propVariant = new PropVariant();
                    this.archive.GetProperty(fileIndex, ItemPropId.kpidPath, ref propVariant);
                    
                    this.entries.Add(new Entry(this.archive, fileIndex)
                    {
                        FileName = (string)propVariant.GetObject()
                    });
                }

                return this.entries;
            }
        }

        private void InitializeAndValidateLibrary()
        {
            if (string.IsNullOrWhiteSpace(this.libraryFilePath))
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll");
                }
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "7z.dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "7z.dll");
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
                this.sevenZipFormat = new SevenZipFormat(this.libraryFilePath);
            }
            catch (Exception e)
            {
                throw new SevenZipException("Unable to initialize SevenZipFormat", e);
            }
        }

        ~ArchiveFile()
        {
            this.Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            this.archiveStream.Dispose();

            if (this.archive != null)
            {
                Marshal.ReleaseComObject(this.archive);
            }

            this.sevenZipFormat.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
