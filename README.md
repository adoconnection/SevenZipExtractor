# SevenZipExtractor
C# wrapper for 7z.dll (included)
Based on code from: http://www.codeproject.com/Articles/27148/C-NET-Interface-for-Zip-Archive-DLLs

Supported formats:
* 7Zip,
* Arj,
* BZip2,
* Cab,
* Chm,
* Compound,
* Cpio,
* Deb,
* GZip,
* Iso,
* Lzh,
* Lzma,
* Nsis,
* Rar,
* Rpm,
* Split,
* Tar,
* Wim,
* Z,
* Zip

NuGet:
```
Install-Package SevenZipExtractor
```

Example:

```cs
using (ArchiveFile archiveFile = new ArchiveFile(@"Archive.ARJ"))
{
    foreach (Entry entry in archiveFile.Entries)
    {
        Console.WriteLine(entry.FileName);
        
        // extract to file
        entry.Extract(entry.FileName);
        
        // extract to stream
        MemoryStream memoryStream = new MemoryStream();
        entry.Extract(memoryStream);
    }
}

```
