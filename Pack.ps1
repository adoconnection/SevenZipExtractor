h:
cd H:\SevenZipExtractorGit\SevenZipExtractor
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild H:\SevenZipExtractorGit\SevenZipExtractor\SevenZipExtractor\SevenZipExtractor.csproj /target:Rebuild /property:Configuration=Release /verbosity:minimal
h:\nuget pack H:\SevenZipExtractorGit\SevenZipExtractor\SevenZipExtractor\SevenZipExtractor.csproj -IncludeReferencedProjects -Prop Configuration=Release
h:\nuget push SevenZipExtractor.1.0.11.nupkg -Source https://www.nuget.org/api/v2/package
