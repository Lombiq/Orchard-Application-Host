$outputDirectory = "build/NuGet"
Set-Alias nuget .nuget/nuget

nuget update -self

If (Test-Path "$outputDirectory") {
	rmdir "$outputDirectory" -Force -Recurse
}
mkdir "$outputDirectory"

nuget pack Lombiq.OrchardAppHost.csproj -Build -Prop Configuration=Release -OutputDirectory "$outputDirectory"