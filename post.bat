SET SolutionDir=D:\Github\Chaos-Server
SET WorkingDir=D:\OneDrive\Documents\Game Addons and Utilities\Darkages\DA TOOLs\ChaosServer\
SET DarkAgesDir=D:\OneDrive\Documents\Game Addons and Utilities\Darkages\DA TOOLs\ChaosServer\KRU\
echo Copying Chaos.exe to %WorkingDir%
xcopy /y /r "%SolutionDir%\Chaos\bin\Debug\Chaos.exe" "%WorkingDir%"
xcopy /y /r "%SolutionDir%\ChaosLauncher\bin\Debug\ChaosLauncher.exe" "%DarkAgesDir%"
xcopy /y /r "%SolutionDir%\MapTool\bin\Debug\MapTool.exe" "%WorkingDir%"