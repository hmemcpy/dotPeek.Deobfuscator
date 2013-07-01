@echo off
setlocal enableextensions

set VERSION=1.1
set PRODUCT=DotPeek
set BASEDIR=JetBrains\%PRODUCT%\v%VERSION%
set PLUGIN=dotPeek.Deobfuscator

set INSTALL_SOURCEDIR=%~dp0\%PLUGIN%

set PER_USER_PLUGINDIR=%LOCALAPPDATA%\%BASEDIR%\plugins\%PLUGIN%

if exist "%PER_USER_PLUGINDIR%" goto do_clean
mkdir "%PER_USER_PLUGINDIR%"

:do_clean
del /q %PER_USER_PLUGINDIR%\*.* 2> NUL

:do_copy
echo Copying files...
copy /y "%INSTALL_SOURCEDIR%\*.dll" "%PER_USER_PLUGINDIR%"

if not exist "%PER_USER_PLUGINDIR%\LICENSES" mkdir "%PER_USER_PLUGINDIR%\LICENSES"
copy /y "%INSTALL_SOURCEDIR%\LICENSES\*.*" "%PER_USER_PLUGINDIR%\LICENSES\"

echo.

REM See https://github.com/citizenmatt/UnblockZoneIdentifier
echo Unblocking downloaded files...
pushd "%PER_USER_PLUGINDIR%"
for /r %%i in (*.dll) do "%~dp0\UnblockZoneIdentifier" "%%i"
popd

:end
pause