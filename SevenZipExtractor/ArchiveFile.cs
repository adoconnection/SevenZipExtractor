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
            SevenZipFormat format;
            this.libraryFilePath = libraryFilePath;

            this.InitializeAndValidateLibrary();

            if (!File.Exists(archiveFilePath))
            {
                throw new SevenZipException("Archive file not found");
            }

            string extension = Path.GetExtension(archiveFilePath);

            if (!string.IsNullOrWhiteSpace(extension) && Formats.ExtensionFormatMapping.ContainsKey(extension.Trim('.').ToLowerInvariant()))
                format = this.GuessFormatFromExtension(archiveFilePath, extension.Trim('.').ToLowerInvariant());
            else
                format = this.GuessFormatFromSignature(archiveFilePath);

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
                string currentArchitecture = IntPtr.Size == 4 ? "x86" : "x64"; // magic check

                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z-" + currentArchitecture + ".dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z-" + currentArchitecture + ".dll");
                }
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "7z-" + currentArchitecture + ".dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "7z-" + currentArchitecture + ".dll");
                }
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentArchitecture, "7z.dll")))
                {
                    this.libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentArchitecture, "7z.dll");
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

        /// <summary>
        /// Get file format by signature (Magic number)
        /// </summary>
        /// <param name="archiveFilePath">Path and filename to extract</param>
        /// <returns>File format</returns>
        private SevenZipFormat GuessFormatFromSignature(string archiveFilePath)
        {
            byte[] archiveFileSignature;

            //SevenZip
            archiveFileSignature = new byte[Formats.SevenZipSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.SevenZipSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.SevenZip;

            //Zip & GZip & ARJ
            archiveFileSignature = new byte[Formats.ZipSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.ZipSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Zip;
            if (Formats.GZipSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.GZip;
            if (Formats.ArjSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Arj;

            //RAR5?
            archiveFileSignature = new byte[Formats.RarFiveSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.RarFiveSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Rar5;
            if (Formats.VhdSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Vhd;

            //RAR5 & DEB & DMG
            archiveFileSignature = new byte[Formats.RarSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.RarSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Rar;
            if (Formats.DebSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Deb;
            if (Formats.DmgSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Dmg;

            //BZip2
            archiveFileSignature = new byte[Formats.BZip2Signature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.BZip2Signature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.BZip2;
            if (Formats.FlvSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Flv;
            if (Formats.SwfSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Swf;

            //TAR
            archiveFileSignature = new byte[Formats.TarSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.TarSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Tar;
            if (Formats.IsoSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Iso;

            //CAB & RPM & XAR & CHM
            archiveFileSignature = new byte[Formats.CabSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 0, archiveFileSignature.Length);
            if (Formats.CabSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Cab;
            if (Formats.RpmSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Rpm;
            if (Formats.XarSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Xar;
            if (Formats.ChmSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Chm;

            //LZH
            archiveFileSignature = new byte[Formats.LzhSignature.Length];
            using (FileStream fileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read))
                fileStream.Read(archiveFileSignature, 2, archiveFileSignature.Length);
            if (Formats.LzhSignature.SequenceEqual(archiveFileSignature))
                return SevenZipFormat.Tar;

            throw new SevenZipException(Path.GetExtension(archiveFilePath) + " is not a known archive type");
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
