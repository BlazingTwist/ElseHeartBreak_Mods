@echo off
set pathToBat=%~dp0
set /p version="No directory provided, input the version number here: "
set targetpath=%pathToBat%firstpersoncam-%version%
mkdir %targetpath%

:PathReady
xcopy "..\..\Mods\FirstPersonCamera\bin\Release\FirstPersonCamera.dll" "%targetpath%\BepInEx\plugins\" /y
xcopy "..\..\Mods\FirstPersonCamera\bin\Release\FirstPersonCamera.pdb" "%targetpath%\BepInEx\plugins\" /y
xcopy "..\..\Mods\FirstPersonCamera\resources\*" "%targetpath%\" /s /e /y
start /WAIT D:\Tools\WinRAR\Rar.exe m -r -ep1 "%targetpath%.rar" "%targetpath%\"
RD "%targetpath%"