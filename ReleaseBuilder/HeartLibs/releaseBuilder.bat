@echo off
set pathToBat=%~dp0
set /p version="No directory provided, input the version number here: "
set targetpath=%pathToBat%heartlibs-%version%
mkdir %targetpath%

:PathReady
xcopy "..\..\Mods\HeartLibs\bin\Release\HeartLibs.dll" "%targetpath%\BepInEx\plugins\" /y
xcopy "..\..\Mods\HeartLibs\bin\Release\HeartLibs.pdb" "%targetpath%\BepInEx\plugins\" /y
start /WAIT D:\Tools\WinRAR\Rar.exe m -r -ep1 "%targetpath%.rar" "%targetpath%\"
RD "%targetpath%"