# SevenZipExtractor
C# wrapper for 7z.dll (x86 and x64 included) 

Based on code from: http://www.codeproject.com/Articles/27148/C-NET-Interface-for-Zip-Archive-DLLs

Supported formats:
* 7Zip
* APM
* Arj
* BZip2
* Cab
* Chm
* Compound
* Cpio
* CramFS
* Deb
* Dll
* Dmg
* Exe
* Fat
* Flv
* Flv
* GZip
* Hfs
* Iso
* Lzh
* Lzma
* Lzma86
* Mach-O
* Mbr
* Mub
* Nsis
* Ntfs
* Ppmd
* Rar
* Rar5
* Rpm
* Split
* SquashFS
* Swf
* Swfc
* Tar
* TE
* Udf
* UEFIc
* UEFIs
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

Examples:

```cs
using (ArchiveFile archiveFile = new ArchiveFile(@"Archive.ARJ"))
{
    archiveFile.Extract("Output"); // extract all
}

```

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


## License
- Source code in this repo is licensed under The MIT License
- 7z binaries license http://www.7-zip.org/license.txt
