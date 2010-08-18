@ECHO OFF

ECHO.
ECHO.     This build script requires the following software to be installed:
ECHO.       - Subversion command-line client
ECHO.       - Microsoft Visual Studio 2008
ECHO.       - Microsoft HTML Help compiler
ECHO.       - Inno Setup 5
ECHO.
ECHO.     You have to commit your work before using this script.
ECHO.     Results will be in the 'Release' directory. Anything currently in
ECHO.     the 'Release' directory may be overwritten.
ECHO.
ECHO.
PAUSE
ECHO.

SET APPDIRECTORY=D:\Applications

CALL "%APPDIRECTORY%\Microsoft Visual Studio 9.0\Common7\Tools\vsvars32.bat"

MKDIR "Release"

svn revert "Source\Core\Properties\AssemblyInfo.cs" > NUL
svn revert "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" > NUL

ECHO.
ECHO Writing SVN log file...
ECHO.
IF EXIST "Release\log.xml" DEL /F /Q "Release\log.xml" > NUL
svn log --xml -r HEAD:1 > "Release\log.xml"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Release\log.xml" GOTO FILEFAIL

ECHO.
ECHO Compiling HTML Help file...
ECHO.
IF EXIST "Build\Refmanual.chm" DEL /F /Q "Build\Refmanual.chm" > NUL
"%APPDIRECTORY%\HTML Help Workshop\hhc" Help\Refmanual.hhp
IF %ERRORLEVEL% NEQ 1 GOTO ERRORFAIL
IF NOT EXIST "Build\Refmanual.chm" GOTO FILEFAIL

ECHO.
ECHO Looking up current repository revision numbers...
ECHO.
IF EXIST "setenv.bat" DEL /F /Q "setenv.bat" > NUL
VersionFromSVN.exe "Source\Core\Properties\AssemblyInfo.cs" "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" -O "setenv.bat"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "setenv.bat" GOTO FILEFAIL

CALL "setenv.bat"
DEL /F /Q "setenv.bat"

ECHO.
ECHO Compiling Doom Builder core...
ECHO.
IF EXIST "Build\Builder.exe" DEL /F /Q "Build\Builder.exe" > NUL
msbuild "Source\Core\Builder.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Builder.exe" GOTO FILEFAIL

ECHO.
ECHO Compiling Doom Builder Modes plugin...
ECHO.
IF EXIST "Build\Plugins\BuilderModes.dll" DEL /F /Q "Build\Plugins\BuilderModes.dll" > NUL
msbuild "Source\Plugins\BuilderModes\BuilderModes.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\BuilderModes.dll" GOTO FILEFAIL

ECHO.
ECHO Building Setup Installer...
ECHO.
IF EXIST "Release\*.exe" DEL /F /Q "Release\*.exe" > NUL
"%APPDIRECTORY%\Inno Setup 5\iscc.exe" "Setup\builder2_setup.iss"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Release\builder2_setup.exe" GOTO FILEFAIL

REN "Release\builder2_setup.exe" builder2_setup_%REVISIONNUMBER%.exe

svn revert "Source\Core\Properties\AssemblyInfo.cs" > NUL
svn revert "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" > NUL

ECHO.
ECHO.     BUILD DONE !
ECHO.
ECHO.     Revision:  %REVISIONNUMBER%
ECHO.
PAUSE > NUL
GOTO LEAVE

:ERRORFAIL
ECHO.
ECHO.     BUILD FAILED (Tool returned error)
ECHO.
PAUSE > NUL
GOTO LEAVE

:FILEFAIL
ECHO.
ECHO.     BUILD FAILED (Output file was not built)
ECHO.
PAUSE > NUL
GOTO LEAVE

:LEAVE
