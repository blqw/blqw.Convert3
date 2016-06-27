
for /r . %%a in (*.nupkg) do (
	del %%a
)
for /r . %%a in (*.nuspec) do (
	nuget pack %%~dpna.csproj -build  -Prop Configuration=Release
)
for /r . %%a in (*.nupkg) do (
	nuget push %%a -source nuget.org
)