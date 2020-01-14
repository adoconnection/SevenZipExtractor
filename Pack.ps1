dotnet build -c Release
dotnet test -c Release 
dotnet pack -c Release -o artifacts SevenZipExtractor\SevenZipExtractor.csproj 
dotnet nuget push artifacts\SevenZipExtractor.1.0.15.nupkg --source https://www.nuget.org/api/v2/package
# h:\nuget pack H:\SevenZipExtractorGit\SevenZipExtractor\SevenZipExtractor\SevenZipExtractor.csproj -IncludeReferencedProjects -Prop Configuration=Release
#h:\nuget push SevenZipExtractor.1.0.15.nupkg -Source https://www.nuget.org/api/v2/package
