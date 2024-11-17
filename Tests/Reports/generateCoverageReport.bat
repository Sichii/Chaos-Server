@RD /S /Q TestResults
@RD /S /Q CoverageReport
dotnet build ..\..\Chaos.sln /p:CreateCoverageReport=true
dotnet %userprofile%\.nuget\packages\reportgenerator\5.4.0\tools\net9.0\ReportGenerator.dll -reports:../**/TestResults/**/*.cobertura.xml -targetdir:CoverageReport -reporttypes:Html_Dark -historydir:CoverageHistory
start "" "CoverageReport\index.html"
pause