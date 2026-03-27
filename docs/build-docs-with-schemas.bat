@echo off
echo Generating JSON schemas...
dotnet run --project "../Tools/SchemaGenerator/SchemaGenerator.csproj" -- "."

echo Building documentation...
docfx docfx.json

echo Documentation build complete!