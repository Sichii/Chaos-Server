@RD /S /Q TestResults
@RD /S /Q CoverageReport
dotnet build ..\..\Chaos.slnx /p:CreateCoverageReport=true

dotnet %userprofile%\.nuget\packages\reportgenerator\5.5.0\tools\net10.0\ReportGenerator.dll ^
  -reports:../**/TestResults/**/*.cobertura.xml ^
  -targetdir:CoverageReport ^
  -reporttypes:Html_Dark ^
  -historydir:CoverageHistory ^
  -assemblyfilters:-Chaos.Schemas

start "" "CoverageReport\index.html"
pause