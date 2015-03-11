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

SET STUDIODIR=c:\Program Files (x86)\Microsoft Visual Studio 9.0
SET HHWDIR=c:\Program Files (x86)\HTML Help Workshop
SET ISSDIR=c:\Program Files (x86)\Inno Setup 5

CALL "%STUDIODIR%\Common7\Tools\vsvars32.bat"

MKDIR "Release"

svn revert "Source\Core\Properties\AssemblyInfo.cs" > NUL
svn revert "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" > NUL

ECHO.
ECHO Writing SVN log file...
ECHO.
IF EXIST "Release\log.xml" DEL /F /Q "Release\log.xml" > NUL
svn log --xml -r HEAD:1496 > "Release\log.xml"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Release\log.xml" GOTO FILEFAIL

ECHO.
ECHO Compiling HTML Help file...
ECHO.
IF EXIST "Build\Refmanual.chm" DEL /F /Q "Build\Refmanual.chm" > NUL
"%HHWDIR%\hhc" Help\Refmanual.hhp
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
ECHO Compiling GZDoom Builder core...
ECHO.
IF EXIST "Build\Builder.exe" DEL /F /Q "Build\Builder.exe" > NUL
IF EXIST "Source\Core\obj" RD /S /Q "Source\Core\obj"
msbuild "Source\Core\Builder.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Builder.exe" GOTO FILEFAIL

ECHO.
ECHO Setting /LARGEADDRESSAWARE flag...
ECHO.
"%STUDIODIR%\VC\bin\editbin.exe" /LARGEADDRESSAWARE "Build\Builder.exe"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL

ECHO.
ECHO Compiling Builder Effects plugin...
ECHO.
IF EXIST "Build\Plugins\BuilderEffects.dll" DEL /F /Q "Build\Plugins\BuilderEffects.dll" > NUL
IF EXIST "Source\Plugins\BuilderEffects\obj" RD /S /Q "Source\Plugins\BuilderEffects\obj"
msbuild "Source\Plugins\BuilderEffects\BuilderEffects.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\BuilderEffects.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Builder Modes plugin...
ECHO.
IF EXIST "Build\Plugins\BuilderModes.dll" DEL /F /Q "Build\Plugins\BuilderModes.dll" > NUL
IF EXIST "Source\Plugins\BuilderModes\obj" RD /S /Q "Source\Plugins\BuilderModes\obj"
msbuild "Source\Plugins\BuilderModes\BuilderModes.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\BuilderModes.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Color Picker plugin...
ECHO.
IF EXIST "Build\Plugins\ColorPicker.dll" DEL /F /Q "Build\Plugins\ColorPicker.dll" > NUL
IF EXIST "Source\Plugins\ColorPicker\obj" RD /S /Q "Source\Plugins\ColorPicker\obj"
msbuild "Source\Plugins\ColorPicker\ColorPicker.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\ColorPicker.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Comments Panel plugin...
ECHO.
IF EXIST "Build\Plugins\CommentsPanel.dll" DEL /F /Q "Build\Plugins\CommentsPanel.dll" > NUL
IF EXIST "Source\Plugins\CommentsPanel\obj" RD /S /Q "Source\Plugins\CommentsPanel\obj"
msbuild "Source\Plugins\CommentsPanel\CommentsPanel.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\CommentsPanel.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Nodes Viewer plugin...
ECHO.
IF EXIST "Build\Plugins\NodesViewer.dll" DEL /F /Q "Build\Plugins\NodesViewer.dll" > NUL
IF EXIST "Source\Plugins\NodesViewer\obj" RD /S /Q "Source\Plugins\NodesViewer\obj"
msbuild "Source\Plugins\NodesViewer\NodesViewer.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\NodesViewer.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Sound Propagation Mode plugin...
ECHO.
IF EXIST "Build\Plugins\SoundPropagationMode.dll" DEL /F /Q "Build\Plugins\SoundPropagationMode.dll" > NUL
IF EXIST "Source\Plugins\SoundPropagationMode\obj" RD /S /Q "Source\Plugins\SoundPropagationMode\obj"
msbuild "Source\Plugins\SoundPropagationMode\SoundPropagation.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\SoundPropagationMode.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Stair Sector Builder plugin...
ECHO.
IF EXIST "Build\Plugins\StairSectorBuilder.dll" DEL /F /Q "Build\Plugins\StairSectorBuilder.dll" > NUL
IF EXIST "Source\Plugins\StairSectorBuilder\obj" RD /S /Q "Source\Plugins\StairSectorBuilder\obj"
msbuild "Source\Plugins\StairSectorBuilder\StairSectorBuilder.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\StairSectorBuilder.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Tag Explorer plugin...
ECHO.
IF EXIST "Build\Plugins\TagExplorer.dll" DEL /F /Q "Build\Plugins\TagExplorer.dll" > NUL
IF EXIST "Source\Plugins\TagExplorer\obj" RD /S /Q "Source\Plugins\TagExplorer\obj"
msbuild "Source\Plugins\TagExplorer\TagExplorer.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\TagExplorer.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Tag Range plugin...
ECHO.
IF EXIST "Build\Plugins\TagRange.dll" DEL /F /Q "Build\Plugins\TagRange.dll" > NUL
IF EXIST "Source\Plugins\TagRange\obj" RD /S /Q "Source\Plugins\TagRange\obj"
msbuild "Source\Plugins\TagRange\TagRange.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\TagRange.dll" GOTO FILEFAIL

ECHO.
ECHO Compiling Visplane Explorer plugin...
ECHO.
IF EXIST "Build\Plugins\VisplaneExplorer.dll" DEL /F /Q "Build\Plugins\VisplaneExplorer.dll" > NUL
IF EXIST "Source\Plugins\VisplaneExplorer\obj" RD /S /Q "Source\Plugins\VisplaneExplorer\obj"
msbuild "Source\Plugins\VisplaneExplorer\VisplaneExplorer.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=x86 /v:minimal
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Plugins\VisplaneExplorer.dll" GOTO FILEFAIL

ECHO.
ECHO Creating changelog...
ECHO.
ChangelogMaker.exe "SVN_Build\log.xml" "Build" "m-x-d"
IF %ERRORLEVEL% NEQ 0 GOTO LOGFAIL

ECHO.
ECHO Building Setup Installer...
ECHO.
IF EXIST "Release\*.exe" DEL /F /Q "Release\*.exe" > NUL
"%ISSDIR%\iscc.exe" "Setup\gzbuilder_setup.iss"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Release\GZDoom Builder Setup.exe" GOTO FILEFAIL

REN "Release\GZDoom Builder Setup.exe" "GZDoom Builder %REVISIONNUMBER% Setup.exe"

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
ECHO.     BUILD FAILED (Tool returned error  %ERRORLEVEL%)
ECHO.
PAUSE > NUL
GOTO LEAVE

:FILEFAIL
ECHO.
ECHO.     BUILD FAILED (Output file was not built)
ECHO.
PAUSE > NUL
GOTO LEAVE

:LOGFAIL
ECHO.
ECHO.     CHANGELOG GENERATION FAILED (Tool returned error %ERRORLEVEL%)
ECHO.
PAUSE > NUL
GOTO LEAVE

:LEAVE
