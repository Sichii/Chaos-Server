@RD /S /Q TestResults
@RD /S /Q CoverageReport
dotnet test "..\..\Chaos.sln" --collect:"XPlat Code Coverage" --settings coverlet.runsettings
dotnet %userprofile%\.nuget\packages\reportgenerator\5.1.22\tools\net6.0\ReportGenerator.dll -reports:TestResults/**/*.opencover.xml -targetdir:CoverageReport -reporttypes:Html_Dark -historydir:CoverageHistory
start "" "CoverageReport\index.html"
pause