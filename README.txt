Compiling the Doom Builder 2 source code
=============================================

The Doom Builder 2 source code uses a tool (VersionFromSVN.exe) to generate the version number from the SVN revision number (due to restrictions with AssemblyInfo.cs we can't use SVN keywords). This tools requires that the Subversion command-line client is installed. (Building the Debug builds works, but leaves you with incorrectly modified AssemblyInfo.cs files)

You can get the Subversion command-line client from http://subversion.tigris.org/getting.html#windows. I have tested with both CollabNet build and the SlikSVN build, both work well.

