# SevenZipExtractor
C# wrapper for 7z.dll (x86 and x64 included) 

[![NuGet](https://img.shields.io/nuget/dt/SevenZipExtractor.svg?style=flat-square)]()
[![NuGet](https://img.shields.io/nuget/v/SevenZipExtractor.svg?style=flat-square)]()

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


## NuGet
```
Install-Package SevenZipExtractor
```

## Examples

#### Extract all
```cs
using (ArchiveFile archiveFile = new ArchiveFile(@"Archive.ARJ"))
{
    archiveFile.Extract("Output"); // extract all
}

```

#### Extract to file or stream
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

#### Guess archive format from files without extensions
```cs
using (ArchiveFile archiveFile = new ArchiveFile(@"c:\random-archive"))
{
    archiveFile.Extract("Output"); 
}
```

#### Guess archive format from streams
```cs
WebRequest request = WebRequest.Create ("http://www.contoso.com/file.aspx?id=12345");
HttpWebResponse response = (HttpWebResponse)request.GetResponse();

using (ArchiveFile archiveFile = new ArchiveFile(response.GetResponseStream())
{
    archiveFile.Extract("Output"); 
}
```

## 7z.dll
7z.dll (x86 and x64) will be added to your BIN folder automatically.


## License
- Source code in this repo is licensed under The MIT License
- 7z binaries license http://www.7-zip.org/license.txt
