
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using System.Diagnostics;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder
{
	internal class MapManager : IDisposable
	{
		#region ================== Constants

		// Map header name in temporary file
		private const string TEMP_MAP_HEADER = "TEMPMAP";
		private const string BUILD_MAP_HEADER = "MAP01";

		// Save modes
		public const int SAVE_NORMAL = 0;
		public const int SAVE_AS = 1;
		public const int SAVE_INTO = 2;
		public const int SAVE_TEST = 3;
		
		#endregion

		#region ================== Variables

		// Status
		private bool changed;
		
		// Map information
		private string filetitle;
		private string filepathname;
		private string temppath;
		private MapSet map;
		private MapSetIO io;
		private MapOptions options;
		private ConfigurationInfo configinfo;
		private Configuration config;
		private DataManager data;
		private EditMode mode;
		private D3DGraphics graphics;
		private WAD tempwad;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string FilePathName { get { return filepathname; } }
		public string FileTitle { get { return filetitle; } }
		public string TempPath { get { return temppath; } }
		public MapOptions Options { get { return options; } }
		public MapSet Map { get { return map; } }
		public EditMode Mode { get { return mode; } }
		public DataManager Data { get { return data; } }
		public bool IsChanged { get { return changed; } set { changed = value; } }
		public bool IsDisposed { get { return isdisposed; } }
		public D3DGraphics Graphics { get { return graphics; } }
		public Configuration Configuration { get { return config; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MapManager()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose
				General.WriteLogLine("Unloading data resources...");
				data.Dispose();
				General.WriteLogLine("Closing temporary file...");
				tempwad.Dispose();
				General.WriteLogLine("Unloading map data...");
				map.Dispose();
				General.WriteLogLine("Stopping edit mode...");
				mode.Dispose();
				General.WriteLogLine("Stopping graphics device...");
				graphics.Dispose();

				// Remove temp file
				General.WriteLogLine("Removing temporary directory...");
				try { Directory.Delete(temppath, true); } catch(Exception e)
				{
					General.WriteLogLine(e.GetType().Name + ": " + e.Message);
					General.WriteLogLine("Failed to remove temporary directory!");
				}

				// We may spend some time to clean things up here
				GC.Collect();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Initialize
		
		// Initializes for a new map
		public bool InitializeNewMap(MapOptions options)
		{
			string tempfile;
			string iointerface;
			
			// Apply settings
			this.filetitle = "unnamed.wad";
			this.filepathname = "";
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Creating new map '" + options.CurrentName + "' with configuration '" + options.ConfigFile + "'");

			// Create temporary path
			temppath = General.MakeTempDirname();
			Directory.CreateDirectory(temppath);
			General.WriteLogLine("Temporary directory:  " + temppath);
			
			// Initiate graphics
			General.WriteLogLine("Initializing graphics device...");
			graphics = new D3DGraphics(General.MainWindow.Display);
			if(!graphics.Initialize()) return false;
			
			// Load game configuration
			General.WriteLogLine("Loading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = General.LoadGameConfiguration(options.ConfigFile);

			// Create map data
			map = new MapSet();
			
			// Create temp wadfile
			tempfile = General.MakeTempFilename(temppath);
			General.WriteLogLine("Creating temporary file: " + tempfile);
			tempwad = new WAD(tempfile);
			
			// Read the map from temp file
			iointerface = config.ReadSetting("formatinterface", "");
			General.WriteLogLine("Initializing map format interface " + iointerface + "...");
			io = MapSetIO.Create(iointerface, tempwad, this);

			// Load data manager
			General.WriteLogLine("Loading data resources...");
			data = new DataManager();
			data.Load(configinfo.Resources, options.Resources);

			// Set default mode
			ChangeMode(typeof(FrozenOverviewMode));

			// Success
			General.WriteLogLine("Map creation done");
			return true;
		}

		// Initializes for an existing map
		public bool InitializeOpenMap(string filepathname, MapOptions options)
		{
			WAD mapwad;
			string tempfile;
			string iointerface;
			DataLocation maplocation;
			
			// Apply settings
			this.filetitle = Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Opening map '" + options.CurrentName + "' with configuration '" + options.ConfigFile + "'");

			// Create temporary path
			temppath = General.MakeTempDirname();
			Directory.CreateDirectory(temppath);
			General.WriteLogLine("Temporary directory:  " + temppath);

			// Initiate graphics
			General.WriteLogLine("Initializing graphics device...");
			graphics = new D3DGraphics(General.MainWindow.Display);
			if(!graphics.Initialize()) return false;

			// Load game configuration
			General.WriteLogLine("Loading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = General.LoadGameConfiguration(options.ConfigFile);

			// Create map data
			map = new MapSet();
			
			// Create temp wadfile
			tempfile = General.MakeTempFilename(temppath);
			General.WriteLogLine("Creating temporary file: " + tempfile);
			tempwad = new WAD(tempfile);
			
			// Now open the map file
			General.WriteLogLine("Opening source file: " + filepathname);
			mapwad = new WAD(filepathname, true);

			// Copy the map lumps to the temp file
			General.WriteLogLine("Copying map lumps to temporary file...");
			CopyLumpsByType(mapwad, options.CurrentName, tempwad, TEMP_MAP_HEADER,
							true, true, true, true);

			// Close the map file
			mapwad.Dispose();
			
			// Read the map from temp file
			iointerface = config.ReadSetting("formatinterface", "");
			General.WriteLogLine("Initializing map format interface " + iointerface + "...");
			io = MapSetIO.Create(iointerface, tempwad, this);
			General.WriteLogLine("Reading map data structures from file...");
			map = io.Read(map, TEMP_MAP_HEADER);

			// Update structures
			map.Update();

			// Load data manager
			General.WriteLogLine("Loading data resources...");
			data = new DataManager();
			maplocation = new DataLocation(DataLocation.RESOURCE_WAD, filepathname, false, false);
			data.Load(configinfo.Resources, options.Resources, maplocation);
			
			// Set default mode
			ChangeMode(typeof(FrozenOverviewMode));

			// Success
			General.WriteLogLine("Map loading done");
			return true;
		}
		
		#endregion

		#region ================== Save

		// Initializes for an existing map
		public bool SaveMap(string filepathname, int savemode)
		{
			MapSet outputset;
			int insertindex;
			string nodebuildername;
			
			// Make a copy of the map data
			outputset = map.Clone();

			// Do we need sidedefs compression?
			if(map.Sidedefs.Count > io.MaxSidedefs)
			{
				// Compress sidedefs
				outputset.CompressSidedefs();

				// Check if it still doesnt fit
				if(map.Sidedefs.Count > io.MaxSidedefs)
				{
					// Problem! Can't save the map like this!
					General.ShowErrorMessage("Unable to save the map: There are too many unique sidedefs!", MessageBoxButtons.OK);
					return false;
				}
			}
			
			// TODO: Check for more limitations
			
			// Write to temporary file
			General.WriteLogLine("Writing map data structures to file...");
			insertindex = tempwad.FindLumpIndex(TEMP_MAP_HEADER);
			if(insertindex == -1) insertindex = 0;
			io.Write(outputset, TEMP_MAP_HEADER, insertindex);
			
			// Get the corresponding nodebuilder
			if(savemode == SAVE_TEST) nodebuildername = configinfo.NodebuilderTest;
				else nodebuildername = configinfo.NodebuilderSave;
			
			// Build the nodes
			if((nodebuildername != null) && (nodebuildername != ""))
				BuildNodes(nodebuildername);
			
			// Suspend data resources
			data.Suspend();


			// Resume data resources
			data.Resume();

			// Success!
			changed = false;
			return true;
		}
		
		#endregion

		#region ================== Nodebuild

		// This builds the nodes in the temproary file with the given configuration name
		private bool BuildNodes(string nodebuildername)
		{
			NodebuilderInfo nodebuilder;
			string tempfile1, tempfile2;
			bool lumpnodebuild, lumpallowempty, lumpscomplete;
			IDictionary maplumps;
			WAD buildwad;
			int srcindex;
			
			// Find the nodebuilder
			nodebuilder = General.GetNodebuilderByName(nodebuildername);
			if(nodebuilder == null)
			{
				// Problem! Can't find that nodebuilder!
				General.ShowWarningMessage("Unable to build the nodes: The configured nodebuilder cannot be found.", MessageBoxButtons.OK);
				return false;
			}
			else
			{
				// Make a temporary file for the nodebuilder
				tempfile1 = General.MakeTempFilename(temppath);
				General.WriteLogLine("Creating temporary build file: " + tempfile1);
				buildwad = new WAD(tempfile1);

				// Copy lumps to buildwad
				General.WriteLogLine("Copying map lumps to temporary build file...");
				CopyLumpsByType(tempwad, TEMP_MAP_HEADER, buildwad, BUILD_MAP_HEADER, true, false, false, true);

				// Close buildwad
				buildwad.Dispose();

				// Does the nodebuilder require an output file?
				if(nodebuilder.HasSpecialOutputFile)
				{
					// Make a temporary output file for the nodebuilder
					tempfile2 = General.MakeTempFilename(temppath);
					General.WriteLogLine("Creating temporary output file: " + tempfile2);
				}
				else
				{
					// Output file is same as input file
					tempfile2 = tempfile1;
				}

				// Run the nodebuilder
				if(nodebuilder.Run(temppath, Path.GetFileName(tempfile1), Path.GetFileName(tempfile2)))
				{
					// Open the output file
					buildwad = new WAD(tempfile2);

					// Find the map header in source
					srcindex = buildwad.FindLumpIndex(BUILD_MAP_HEADER);
					if(srcindex > -1)
					{
						// Go for all the map lump names
						lumpscomplete = true;
						maplumps = config.ReadSetting("maplumpnames", new Hashtable());
						foreach(DictionaryEntry ml in maplumps)
						{
							// Read lump settings from map config
							lumpnodebuild = config.ReadSetting("maplumpnames." + ml.Key + ".nodebuild", false);
							lumpallowempty = config.ReadSetting("maplumpnames." + ml.Key + ".allowempty", false);

							// Check if this lump should exist
							if(lumpnodebuild && !lumpallowempty)
							{
								// Find the lump in the source
								if(buildwad.FindLump(ml.Key.ToString(), srcindex, srcindex + maplumps.Count + 2) == null)
								{
									// Missing a lump!
									lumpscomplete = false;
									break;
								}
							}
						}
					}
					else
					{
						// Cannot find header
						lumpscomplete = false;
					}
					
					// Output lumps complete?
					if(lumpscomplete)
					{
						// Copy nodebuilder lumps to temp file
						General.WriteLogLine("Copying nodebuilder lumps to temporary file...");
						CopyLumpsByType(buildwad, BUILD_MAP_HEADER, tempwad, TEMP_MAP_HEADER, false, false, true, false);
					}
					else
					{
						// Problem! Nodebuilder did not build the lumps!
						General.ShowWarningMessage("Unable to build the nodes: The nodebuilder failed to build the expected data structures.", MessageBoxButtons.OK);
					}
					
					// Done with the build wad
					buildwad.Dispose();
					
					// Remove temp files
					General.WriteLogLine("Removing temporary files...");
					if(File.Exists(tempfile1)) File.Delete(tempfile1);
					if(File.Exists(tempfile2)) File.Delete(tempfile2);
					return lumpscomplete;
				}
				else
				{
					// Remove temp files
					General.WriteLogLine("Removing temporary files...");
					if(File.Exists(tempfile1)) File.Delete(tempfile1);
					if(File.Exists(tempfile2)) File.Delete(tempfile2);
					return false;
				}
			}
		}

		#endregion

		#region ================== Methods

		// This copies specific map lumps from one WAD to another
		private void CopyLumpsByType(WAD source, string sourcemapname,
									 WAD target, string targetmapname,
									 bool copyrequired, bool copyblindcopy,
									 bool copynodebuild, bool copyscript)
		{
			bool lumprequired, lumpblindcopy, lumpnodebuild;
			string lumpscript;
			int srcindex, tgtindex;
			IDictionary maplumps;
			Lump lump, newlump;
			
			// Find the map header in target
			tgtindex = target.FindLumpIndex(targetmapname);
			if(tgtindex > -1)
			{
				// Remove the lumps from target
				RemoveLumpsByType(target, targetmapname, copyrequired,
							copyblindcopy, copynodebuild, copyscript);
			}
			else
			{
				// If this header doesnt exists in the target
				// then insert at the end of the target
				tgtindex = target.Lumps.Count;
			}
			
			// Find the map header in source
			srcindex = source.FindLumpIndex(sourcemapname);
			if(srcindex > -1)
			{
				// Copy the map header from source to target
				newlump = target.Insert(targetmapname, tgtindex++, source.Lumps[srcindex].Length);
				source.Lumps[srcindex].CopyTo(newlump);
				
				// Go for all the map lump names
				maplumps = config.ReadSetting("maplumpnames", new Hashtable());
				foreach(DictionaryEntry ml in maplumps)
				{
					// Read lump settings from map config
					lumprequired = config.ReadSetting("maplumpnames." + ml.Key + ".required", false);
					lumpblindcopy = config.ReadSetting("maplumpnames." + ml.Key + ".blindcopy", false);
					lumpnodebuild = config.ReadSetting("maplumpnames." + ml.Key + ".nodebuild", false);
					lumpscript = config.ReadSetting("maplumpnames." + ml.Key + ".script", "");

					// Check if this lump should be copied
					if((lumprequired && copyrequired) || (lumpblindcopy && copyblindcopy) ||
					   (lumpnodebuild && copynodebuild) || ((lumpscript != "") && copyscript))
					{
						// Find the lump in the source
						lump = source.FindLump(ml.Key.ToString(), srcindex, srcindex + maplumps.Count + 2);
						if(lump != null)
						{
							// Copy the lump to the target
							//General.WriteLogLine(ml.Key.ToString() + " copying");
							newlump = target.Insert(ml.Key.ToString(), tgtindex++, lump.Length);
							lump.CopyTo(newlump);
						}
						else
						{
							General.WriteLogLine("WARNING: " + ml.Key.ToString() + " should be copied but was not found!");
						}
					}
				}
			}
		}
		
		// This copies specific map lumps from one WAD to another
		private void RemoveLumpsByType(WAD source, string sourcemapname,
									   bool removerequired, bool removeblindcopy,
									   bool removenodebuild, bool removescript)
		{
			bool lumprequired, lumpblindcopy, lumpnodebuild;
			string nextlumpname, lumpscript;
			int index;
			
			// Find the map header in target
			index = source.FindLumpIndex(sourcemapname);
			if(index > -1)
			{
				// Remove the header from target
				source.RemoveAt(index);

				// Get the name of the next lump
				if(index < source.Lumps.Count) nextlumpname = source.Lumps[index].Name;
				else nextlumpname = "";

				// Do we recognize this lump type?
				while(config.ReadSetting("maplumpnames." + nextlumpname, (IDictionary)null) != null)
				{
					// Read lump settings from map config
					lumprequired = config.ReadSetting("maplumpnames." + nextlumpname + ".required", false);
					lumpblindcopy = config.ReadSetting("maplumpnames." + nextlumpname + ".blindcopy", false);
					lumpnodebuild = config.ReadSetting("maplumpnames." + nextlumpname + ".nodebuild", false);
					lumpscript = config.ReadSetting("maplumpnames." + nextlumpname + ".script", "");

					// Check if this lump will be removed from target
					if((lumprequired && removerequired) || (lumpblindcopy && removeblindcopy) ||
					   (lumpnodebuild && removenodebuild) || ((lumpscript != "") && removescript))
					{
						// Then remove it from target
						//General.WriteLogLine(nextlumpname + " removing");
						source.RemoveAt(index);
					}
					else
					{
						// Advance to the next lump
						index++;
					}

					// Get the name of the next lump
					if(index < source.Lumps.Count) nextlumpname = source.Lumps[index].Name;
					else nextlumpname = "";
				}
			}
		}

		// This changes editing mode
		public void ChangeMode(Type modetype, params object[] args)
		{
			// Dispose current mode
			if(mode != null) mode.Dispose();
			
			// Create a new mode
			General.WriteLogLine("Switching edit mode to " + modetype.Name + "...");
			mode = EditMode.Create(modetype, args);
			
			// Redraw the display
			General.MainWindow.RedrawDisplay();
		}
		
		#endregion
	}
}
