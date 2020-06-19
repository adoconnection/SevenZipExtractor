using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SevenZipExtractor
{
    public class ArchiveFile : IDisposable
    {
        private readonly IInArchive archive;
        private readonly InStreamWrapper archiveStream;
        private IList<Entry> entries;

        private string libraryFilePath;
        private SevenZipHandle sevenZipHandle;

        public ArchiveFile(string archiveFilePath, string libraryFilePath = null)
        {
            this.libraryFilePath = libraryFilePath;

            InitializeAndValidateLibrary();

            if (!File.Exists(archiveFilePath)) throw new SevenZipException("Archive file not found");

            var extension = Path.GetExtension(archiveFilePath);

            if (GuessFormatFromExtension(extension, out var format))
            {
                // great
            }
            else if (GuessFormatFromSignature(archiveFilePath, out format))
            {
                // success
            }
            else
                throw new SevenZipException(Path.GetFileName(archiveFilePath) + " is not a known archive type");

            archive = sevenZipHandle.CreateInArchive(Formats.FormatGuidMapping[format]);
            archiveStream = new InStreamWrapper(File.OpenRead(archiveFilePath));
        }

        public ArchiveFile(Stream archiveStream, SevenZipFormat? format = null, string libraryFilePath = null)
        {
            this.libraryFilePath = libraryFilePath;

            InitializeAndValidateLibrary();

            if (archiveStream == null) throw new SevenZipException("archiveStream is null");

            if (format == null)
            {
                if (GuessFormatFromSignature(archiveStream, out var guessedFormat))
                    format = guessedFormat;
                else
                    throw new SevenZipException("Unable to guess format automatically");
            }

            archive = sevenZipHandle.CreateInArchive(Formats.FormatGuidMapping[format.Value]);
            this.archiveStream = new InStreamWrapper(archiveStream);
        }

        public IList<Entry> Entries
        {
            get
            {
                if (entries != null) return entries;

                ulong checkPos = 32 * 1024;
                var open = archive.Open(archiveStream, ref checkPos, null);

                if (open != 0) throw new SevenZipException("Unable to open archive");

                var itemsCount = archive.GetNumberOfItems();

                entries = new List<Entry>();

                for (uint fileIndex = 0; fileIndex < itemsCount; fileIndex++)
                {
                    var fileName = GetProperty<string>(fileIndex, ItemPropId.kpidPath);
                    var isFolder = GetProperty<bool>(fileIndex, ItemPropId.kpidIsFolder);
                    var isEncrypted = GetProperty<bool>(fileIndex, ItemPropId.kpidEncrypted);
                    var size = GetProperty<ulong>(fileIndex, ItemPropId.kpidSize);
                    var packedSize = GetProperty<ulong>(fileIndex, ItemPropId.kpidPackedSize);
                    var creationTime = GetProperty<DateTime>(fileIndex, ItemPropId.kpidCreationTime);
                    var lastWriteTime = GetProperty<DateTime>(fileIndex, ItemPropId.kpidLastWriteTime);
                    var lastAccessTime = GetProperty<DateTime>(fileIndex, ItemPropId.kpidLastAccessTime);
                    var crc = GetProperty<uint>(fileIndex, ItemPropId.kpidCRC);
                    var attributes = GetProperty<uint>(fileIndex, ItemPropId.kpidAttributes);
                    var comment = GetProperty<string>(fileIndex, ItemPropId.kpidComment);
                    var hostOS = GetProperty<string>(fileIndex, ItemPropId.kpidHostOS);
                    var method = GetProperty<string>(fileIndex, ItemPropId.kpidMethod);

                    var isSplitBefore = GetProperty<bool>(fileIndex, ItemPropId.kpidSplitBefore);
                    var isSplitAfter = GetProperty<bool>(fileIndex, ItemPropId.kpidSplitAfter);

                    entries.Add(new Entry(archive, fileIndex)
                    {
                        FileName = fileName,
                        IsFolder = isFolder,
                        IsEncrypted = isEncrypted,
                        Size = size,
                        PackedSize = packedSize,
                        CreationTime = creationTime,
                        LastWriteTime = lastWriteTime,
                        LastAccessTime = lastAccessTime,
                        CRC = crc,
                        Attributes = attributes,
                        Comment = comment,
                        HostOS = hostOS,
                        Method = method,
                        IsSplitBefore = isSplitBefore,
                        IsSplitAfter = isSplitAfter
                    });
                }

                return entries;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Extract(string outputFolder, bool overwrite = false)
        {
            Extract(entry =>
            {
                var fileName = Path.Combine(outputFolder, entry.FileName);

                if (entry.IsFolder) return fileName;

                if (!File.Exists(fileName) || overwrite) return fileName;

                return null;
            });
        }

        public void Extract(Func<Entry, string> getOutputPath)
        {
            IList<Stream> fileStreams = new List<Stream>();

            try
            {
                foreach (var entry in Entries)
                {
                    var outputPath = getOutputPath(entry);

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

                    var directoryName = Path.GetDirectoryName(outputPath);

                    if (!string.IsNullOrWhiteSpace(directoryName)) Directory.CreateDirectory(directoryName);

                    fileStreams.Add(File.Create(outputPath));
                }

                archive.Extract(null, 0xFFFFFFFF, 0, new ArchiveStreamsCallback(fileStreams));
            }
            finally
            {
                foreach (var stream in fileStreams)
                    stream?.Dispose();
            }
        }

        private T GetProperty<T>(uint fileIndex, ItemPropId name)
        {
            var propVariant = new PropVariant();
            archive.GetProperty(fileIndex, name, ref propVariant);

            var result = propVariant.VarType != VarEnum.VT_EMPTY
                ? (T) propVariant.GetObject()
                : default;

            propVariant.Clear();

            return result;
        }

        private void InitializeAndValidateLibrary()
        {
            if (string.IsNullOrWhiteSpace(libraryFilePath))
            {
                var currentArchitecture = IntPtr.Size == 4 ? "x86" : "x64"; // magic check

                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "7z-" + currentArchitecture + ".dll")))
                    libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "7z-" + currentArchitecture + ".dll");
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin",
                    "7z-" + currentArchitecture + ".dll")))
                    libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin",
                        "7z-" + currentArchitecture + ".dll");
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentArchitecture,
                    "7z.dll")))
                    libraryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentArchitecture,
                        "7z.dll");
                else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "7-Zip", "7z.dll")))
                    libraryFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        "7-Zip", "7z.dll");
            }

            if (string.IsNullOrWhiteSpace(libraryFilePath)) throw new SevenZipException("libraryFilePath not set");

            if (!File.Exists(libraryFilePath)) throw new SevenZipException("7z.dll not found");

            try
            {
                sevenZipHandle = new SevenZipHandle(libraryFilePath);
            }
            catch (Exception e)
            {
                throw new SevenZipException("Unable to initialize SevenZipHandle", e);
            }
        }

        private bool GuessFormatFromExtension(string fileExtension, out SevenZipFormat format)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                format = SevenZipFormat.Undefined;
                return false;
            }

            fileExtension = fileExtension.TrimStart('.').Trim().ToLowerInvariant();

            if (fileExtension.Equals("rar"))
            {
                // 7z has different GUID for Pre-RAR5 and RAR5, but they have both same extension (.rar)
                // If it is [0x52 0x61 0x72 0x21 0x1A 0x07 0x01 0x00] then file is RAR5 otherwise RAR.
                // https://www.rarlab.com/technote.htm

                // We are unable to guess right format just by looking at extension and have to check signature

                format = SevenZipFormat.Undefined;
                return false;
            }

            if (!Formats.ExtensionFormatMapping.ContainsKey(fileExtension))
            {
                format = SevenZipFormat.Undefined;
                return false;
            }

            format = Formats.ExtensionFormatMapping[fileExtension];
            return true;
        }

        private bool GuessFormatFromSignature(string filePath, out SevenZipFormat format)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                return GuessFormatFromSignature(fileStream, out format);
        }

        private bool GuessFormatFromSignature(Stream stream, out SevenZipFormat format)
        {
            var longestSignature = Formats.FileSignatures.Values.OrderByDescending(v => v.Length).First().Length;

            var archiveFileSignature = new byte[longestSignature];
            var bytesRead = stream.Read(archiveFileSignature, 0, longestSignature);

            stream.Position -= bytesRead; // go back o beginning

            if (bytesRead != longestSignature)
            {
                format = SevenZipFormat.Undefined;
                return false;
            }

            foreach (var pair in Formats.FileSignatures)
                if (archiveFileSignature.Take(pair.Value.Length).SequenceEqual(pair.Value))
                {
                    format = pair.Key;
                    return true;
                }

            format = SevenZipFormat.Undefined;
            return false;
        }

        ~ArchiveFile()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            archiveStream?.Dispose();

            if (archive != null) Marshal.ReleaseComObject(archive);

            sevenZipHandle?.Dispose();
        }
    }
}