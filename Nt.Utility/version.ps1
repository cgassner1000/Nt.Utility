# update-version.ps1
$tag = git describe --tags --abbrev=0
$version = $tag -replace '^v', ''
$csprojPath = "Nt.Utility.csproj"

[xml]$csproj = Get-Content $csprojPath
$csproj.Project.PropertyGroup.Version = $version
$csproj.Save($csprojPath)