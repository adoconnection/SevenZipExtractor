# SevenZipExtractor
C# wrapper for 7z.dll (x86 and x64 included) 
- .NET Standard 2.0
- .NET Framework 4.5

[![NuGet](https://img.shields.io/nuget/dt/SevenZipExtractor.svg?style=flat-square)](https://www.nuget.org/packages/SevenZipExtractor)
[![NuGet](https://img.shields.io/nuget/v/SevenZipExtractor.svg?style=flat-square)](https://www.nuget.org/packages/SevenZipExtractor)

Hooray! 🎉✨ 100 000 downloads, like, subscribe, repost :)

Every single star makes maintainer happy! ⭐

## NuGet
```
Install-Package SevenZipExtractor
```

## Supported formats:
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
* Vhd (?)
* Wim
* Xar
* XZ
* Z
* Zip




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

## Wiki
* [Extracting from solid archives](https://github.com/adoconnection/SevenZipExtractor/wiki/Extracting-from-solid-archives)
* [Extract tar.gz, tag.xz](https://github.com/adoconnection/SevenZipExtractor/wiki/Extract-tar.gz,-tag.xz)



## 7z.dll
7z.dll (x86 and x64) will be added to your BIN folder automatically.


## License
- Based on code from: http://www.codeproject.com/Articles/27148/C-NET-Interface-for-Zip-Archive-DLLs
- Source code in this repo is licensed under The MIT License
- 7z binaries license http://www.7-zip.org/license.txt



## Changelog
1.0.15 / 2020.01.14
- .NETStandard 2.0 support PR#38

1.0.14
- Entry.Extrat - preserveTimestamp is true by default #34
- Dynamic operations can only be performed in homogenous AppDomain" #36
