@echo off
echo Building all mods...

for /r %%F in (*.csproj) do (
    if /I not "%%~nxF"=="template.csproj" (
        echo Building %%F...
        dotnet build "%%F" -c Release
    )
)

echo Build complete.
