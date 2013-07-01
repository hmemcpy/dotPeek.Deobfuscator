@echo off
setlocal enableextensions

set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Clean
echo Cleaning...
del /q src\dotPeek.Deobfuscator\bin\Release\*

REM Build DotPeek 1.1 version
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\dotPeek.Deobfuscator.sln /p:Configuration="%config%" /t:Clean,Rebuild /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
mkdir install\dotPeek.Deobfuscator 2> NUL
mkdir install\dotPeek.Deobfuscator\LICENSES 2> NUL
copy /y src\dotPeek.Deobfuscator\bin\Release\*.dll install\dotPeek.Deobfuscator\
copy /y src\de4dot\Release\LICENSES\*.* install\dotPeek.Deobfuscator\LICENSES