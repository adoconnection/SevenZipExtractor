using System;
using System.Collections.Generic;

namespace SevenZipExtractor
{
    public class Formats
    {
        internal static readonly Dictionary<string, KnownSevenZipFormat> ExtensionFormatMapping =
            new Dictionary<string, KnownSevenZipFormat>

            {{"7z",     KnownSevenZipFormat.SevenZip},
             {"gz",     KnownSevenZipFormat.GZip},
             {"tar",    KnownSevenZipFormat.Tar},
             {"rar",    KnownSevenZipFormat.Rar},
             {"zip",    KnownSevenZipFormat.Zip},
             {"lzma",   KnownSevenZipFormat.Lzma},
             {"lzh",    KnownSevenZipFormat.Lzh},
             {"arj",    KnownSevenZipFormat.Arj},
             {"bz2",    KnownSevenZipFormat.BZip2},
             {"cab",    KnownSevenZipFormat.Cab},
             {"chm",    KnownSevenZipFormat.Chm},
             {"deb",    KnownSevenZipFormat.Deb},
             {"iso",    KnownSevenZipFormat.Iso},
             {"rpm",    KnownSevenZipFormat.Rpm},
             {"wim",    KnownSevenZipFormat.Wim},
             {"udf",    KnownSevenZipFormat.Udf},
             {"mub",    KnownSevenZipFormat.Mub},
             {"xar",    KnownSevenZipFormat.Xar},
             {"hfs",    KnownSevenZipFormat.Hfs},
             {"dmg",    KnownSevenZipFormat.Dmg},
             {"Z",      KnownSevenZipFormat.Lzw},
             {"xz",     KnownSevenZipFormat.XZ},
             {"flv",    KnownSevenZipFormat.Flv},
             {"swf",    KnownSevenZipFormat.Swf},
             {"exe",    KnownSevenZipFormat.PE},
             {"dll",    KnownSevenZipFormat.PE},
             {"vhd",    KnownSevenZipFormat.Vhd}
        };

        internal static Dictionary<KnownSevenZipFormat, Guid> FormatGuidMapping = new Dictionary<KnownSevenZipFormat, Guid>
        {
            {KnownSevenZipFormat.SevenZip, new Guid("23170f69-40c1-278a-1000-000110070000")},
            {KnownSevenZipFormat.Arj, new Guid("23170f69-40c1-278a-1000-000110040000")},
            {KnownSevenZipFormat.BZip2, new Guid("23170f69-40c1-278a-1000-000110020000")},
            {KnownSevenZipFormat.Cab, new Guid("23170f69-40c1-278a-1000-000110080000")},
            {KnownSevenZipFormat.Chm, new Guid("23170f69-40c1-278a-1000-000110e90000")},
            {KnownSevenZipFormat.Compound, new Guid("23170f69-40c1-278a-1000-000110e50000")},
            {KnownSevenZipFormat.Cpio, new Guid("23170f69-40c1-278a-1000-000110ed0000")},
            {KnownSevenZipFormat.Deb, new Guid("23170f69-40c1-278a-1000-000110ec0000")},
            {KnownSevenZipFormat.GZip, new Guid("23170f69-40c1-278a-1000-000110ef0000")},
            {KnownSevenZipFormat.Iso, new Guid("23170f69-40c1-278a-1000-000110e70000")},
            {KnownSevenZipFormat.Lzh, new Guid("23170f69-40c1-278a-1000-000110060000")},
            {KnownSevenZipFormat.Lzma, new Guid("23170f69-40c1-278a-1000-0001100a0000")},
            {KnownSevenZipFormat.Nsis, new Guid("23170f69-40c1-278a-1000-000110090000")},
            {KnownSevenZipFormat.Rar, new Guid("23170f69-40c1-278a-1000-000110030000")},
            {KnownSevenZipFormat.Rpm, new Guid("23170f69-40c1-278a-1000-000110eb0000")},
            {KnownSevenZipFormat.Split, new Guid("23170f69-40c1-278a-1000-000110ea0000")},
            {KnownSevenZipFormat.Tar, new Guid("23170f69-40c1-278a-1000-000110ee0000")},
            {KnownSevenZipFormat.Wim, new Guid("23170f69-40c1-278a-1000-000110e60000")},
            {KnownSevenZipFormat.Lzw, new Guid("23170f69-40c1-278a-1000-000110050000")},
            {KnownSevenZipFormat.Zip, new Guid("23170f69-40c1-278a-1000-000110010000")},
            {KnownSevenZipFormat.Udf, new Guid("23170f69-40c1-278a-1000-000110E00000")},
            {KnownSevenZipFormat.Xar, new Guid("23170f69-40c1-278a-1000-000110E10000")},
            {KnownSevenZipFormat.Mub, new Guid("23170f69-40c1-278a-1000-000110E20000")},
            {KnownSevenZipFormat.Hfs, new Guid("23170f69-40c1-278a-1000-000110E30000")},
            {KnownSevenZipFormat.Dmg, new Guid("23170f69-40c1-278a-1000-000110E40000")},
            {KnownSevenZipFormat.XZ, new Guid("23170f69-40c1-278a-1000-0001100C0000")},
            {KnownSevenZipFormat.Mslz, new Guid("23170f69-40c1-278a-1000-000110D50000")},
            {KnownSevenZipFormat.PE, new Guid("23170f69-40c1-278a-1000-000110DD0000")},
            {KnownSevenZipFormat.Elf, new Guid("23170f69-40c1-278a-1000-000110DE0000")},
            {KnownSevenZipFormat.Swf, new Guid("23170f69-40c1-278a-1000-000110D70000")},
            {KnownSevenZipFormat.Vhd, new Guid("23170f69-40c1-278a-1000-000110DC0000")}
        };
    }
}