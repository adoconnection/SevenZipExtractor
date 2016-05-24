# SevenZipExtractor
C# wrapper for 7z.dll (included) 

Based on code from: http://www.codeproject.com/Articles/27148/C-NET-Interface-for-Zip-Archive-DLLs

Supported formats:
* 7Zip
* Arj
* BZip2
* Cab
* Chm
* Compound
* Cpio
* Deb
* Dll
* Dmg
* Exe
* Flv
* GZip
* Hfs
* Iso
* Lzh
* Lzma
* Mub
* Nsis
* Rar
* Rpm
* Split
* Swf
* Tar
* Udf
* Vhd
* Wim
* Xar
* XZ
* Z
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

## 7z.dll
7z-x64.dll and 7z-x86.dll has to be in your binaries folder. In Solution Explorer set *Build action = Copy always* for both files
