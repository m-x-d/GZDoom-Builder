@ECHO OFF

ECHO.
ECHO.     This build script requires the following software to be installed:
ECHO.       - Subversion command-line client
ECHO.       - Microsoft Visual Studio 2008
ECHO.       - Microsoft HTML Help compiler
ECHO.       - Inno Setup 5
ECHO.
ECHO.     You have to commit your work before using this script.
ECHO.
ECHO.
PAUSE
ECHO.

CALL "%programfiles%\Microsoft Visual Studio 9.0\Common7\Tools\vsvars32.bat"

MKDIR "Release"

svn revert "Source\Core\Properties\AssemblyInfo.cs"
svn revert "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs"

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
"%programfiles%\HTML Help Workshop\hhc" Help\Refmanual.hhp
IF %ERRORLEVEL% NEQ 1 GOTO ERRORFAIL
IF NOT EXIST "Build\Refmanual.chm" GOTO FAIL

ECHO.
ECHO Looking up current repository revision numbers...
ECHO.
VersionFromSVN.exe "Source\Core\Properties\AssemblyInfo.cs"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
VersionFromSVN.exe "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL

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
IF EXIST "Release\builder2_setup.exe" DEL /F /Q "Release\builder2_setup.exe" > NUL
"%programfiles%\Inno Setup 5\iscc.exe" "Setup\builder2_setup.iss"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Release\builder2_setup.exe" GOTO FILEFAIL

svn revert "Source\Core\Properties\AssemblyInfo.cs"
svn revert "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs"

ECHO.
ECHO.     BUILD DONE !
ECHO.
GOTO LEAVE

:ERRORFAIL
ECHO.
ECHO.     BUILD FAILED (Tool returned error)
ECHO.
PAUSE
GOTO LEAVE

:FILEFAIL
ECHO.
ECHO.     BUILD FAILED (Output file was not built)
ECHO.
PAUSE
GOTO LEAVE

:LEAVE
