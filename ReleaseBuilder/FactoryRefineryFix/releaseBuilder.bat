@echo off
set pathToBat=%~dp0
set /p version="No directory provided, input the version number here: "
set targetpath=%pathToBat%factoryrefineryfix-%version%
mkdir %targetpath%

:PathReady
xcopy "..\..\Mods\FactoryRefineryFix\bin\Release\FactoryRefineryFix.dll" "%targetpath%\BepInEx\plugins\" /y
xcopy "..\..\Mods\FactoryRefineryFix\bin\Release\FactoryRefineryFix.pdb" "%targetpath%\BepInEx\plugins\" /y
start /WAIT D:\Tools\WinRAR\Rar.exe m -r -ep1 "%targetpath%.rar" "%targetpath%\"
RD "%targetpath%"