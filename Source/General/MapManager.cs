
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
using System.Diagnostics;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Plugins;

#endregion

namespace CodeImp.DoomBuilder
{
	public class MapManager
	{
		#region ================== Constants

		// Map header name in temporary file
		internal const string TEMP_MAP_HEADER = "TEMPMAP";
		internal const string BUILD_MAP_HEADER = "MAP01";
		public const string CONFIG_MAP_HEADER = "~MAP";

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
		private bool cancelmodechange;

		// Main objects
		private MapSet map;
		private MapSetIO io;
		private MapOptions options;
		private ConfigurationInfo configinfo;
		private GameConfiguration config;
		private DataManager data;
		private EditMode mode;
		private EditMode newmode;
		private Type prevmode;
		private Type prevstablemode;
		private D3DDevice graphics;
		private Renderer2D renderer2d;
		private Renderer3D renderer3d;
		private WAD tempwad;
		private GridSetup grid;
		private UndoManager undoredo;
		private CopyPasteManager copypaste;
		private Launcher launcher;
		private ThingsFilter thingsfilter;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string FilePathName { get { return filepathname; } }
		public string FileTitle { get { return filetitle; } }
		public string TempPath { get { return temppath; } }
		public bool CancelModeChange { get { return cancelmodechange; } set { cancelmodechange |= value; } }
		internal MapOptions Options { get { return options; } }
		public MapSet Map { get { return map; } }
		public EditMode Mode { get { return mode; } }
		public EditMode NewMode { get { return newmode; } }
		public Type PreviousMode { get { return prevmode; } }
		public Type PreviousStableMode { get { return prevstablemode; } }
		public DataManager Data { get { return data; } }
		public bool IsChanged { get { return changed; } set { changed |= value; } }
		public bool IsDisposed { get { return isdisposed; } }
		internal D3DDevice Graphics { get { return graphics; } }
		public IRenderer2D Renderer2D { get { return renderer2d; } }
		public IRenderer3D Renderer3D { get { return renderer3d; } }
		public GameConfiguration Config { get { return config; } }
		internal ConfigurationInfo ConfigSettings { get { return configinfo; } }
		public GridSetup Grid { get { return grid; } }
		public UndoManager UndoRedo { get { return undoredo; } }
		public IMapSetIO FormatInterface { get { return io; } }
		internal Launcher Launcher { get { return launcher; } }
		public ThingsFilter ThingsFilter { get { return thingsfilter; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapManager()
		{
			// We have no destructor
			GC.SuppressFinalize(this);

			// Create temporary path
			temppath = General.MakeTempDirname();
			Directory.CreateDirectory(temppath);
			General.WriteLogLine("Temporary directory:  " + temppath);

			// Basic objects
			grid = new GridSetup();
			undoredo = new UndoManager();
			copypaste = new CopyPasteManager();
			launcher = new Launcher(this);
			thingsfilter = new NullThingsFilter();
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Change to no mode
				ChangeMode((EditMode)null);
				
				// Unbind any methods
				General.Actions.UnbindMethods(this);

				// Dispose
				if(grid != null) grid.Dispose();
				if(launcher != null) launcher.Dispose();
				if(copypaste != null) copypaste.Dispose();
				if(undoredo != null) undoredo.Dispose();
				General.WriteLogLine("Unloading data resources...");
				if(data != null) data.Dispose();
				General.WriteLogLine("Closing temporary file...");
				if(tempwad != null) tempwad.Dispose();
				General.WriteLogLine("Unloading map data...");
				if(map != null) map.Dispose();
				General.WriteLogLine("Stopping graphics device...");
				if(renderer2d != null) renderer2d.Dispose();
				if(renderer3d != null) renderer3d.Dispose();
				if(graphics != null) graphics.Dispose();
				
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

		#region ================== New / Open
		
		// Initializes for a new map
		internal bool InitializeNewMap(MapOptions options)
		{
			string tempfile;
			
			// Apply settings
			this.filetitle = "unnamed.wad";
			this.filepathname = "";
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Creating new map '" + options.CurrentName + "' with configuration '" + options.ConfigFile + "'");

			// Initiate graphics
			General.WriteLogLine("Initializing graphics device...");
			graphics = new D3DDevice(General.MainWindow.Display);
			if(!graphics.Initialize()) return false;
			
			// Create renderers
			renderer2d = new Renderer2D(graphics);
			renderer3d = new Renderer3D(graphics);
			
			// Load game configuration
			General.WriteLogLine("Loading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = new GameConfiguration(General.LoadGameConfiguration(options.ConfigFile));
			configinfo.ApplyDefaults();
			General.Plugins.GameConfigurationChanged();
			
			// Create map data
			map = new MapSet();
			
			// Create temp wadfile
			tempfile = General.MakeTempFilename(temppath);
			General.WriteLogLine("Creating temporary file: " + tempfile);
			tempwad = new WAD(tempfile);
			
			// Read the map from temp file
			General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
			io = MapSetIO.Create(config.FormatInterface, tempwad, this);

			// Create required lumps
			General.WriteLogLine("Creating map data structures...");
			tempwad.Insert(TEMP_MAP_HEADER, 0, 0);
			io.Write(map, TEMP_MAP_HEADER, 1);
			CreateRequiredLumps(tempwad, TEMP_MAP_HEADER);
			
			// Update structures
			map.Update();
			thingsfilter.Update();
			
			// Load data manager
			General.WriteLogLine("Loading data resources...");
			data = new DataManager();
			data.Load(configinfo.Resources, options.Resources);

			// Bind any methods
			General.Actions.BindMethods(this);

			// Set default mode
			ChangeMode("VerticesMode");
			ClassicMode cmode = (mode as ClassicMode);
			cmode.SetZoom(0.5f);

			// Success
			General.WriteLogLine("Map creation done");
			return true;
		}

		// Initializes for an existing map
		internal bool InitializeOpenMap(string filepathname, MapOptions options)
		{
			WAD mapwad;
			string tempfile;
			DataLocation maplocation;
			
			// Apply settings
			this.filetitle = Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Opening map '" + options.CurrentName + "' with configuration '" + options.ConfigFile + "'");

			// Initiate graphics
			General.WriteLogLine("Initializing graphics device...");
			graphics = new D3DDevice(General.MainWindow.Display);
			if(!graphics.Initialize()) return false;

			// Create renderers
			renderer2d = new Renderer2D(graphics);
			renderer3d = new Renderer3D(graphics);

			// Load game configuration
			General.WriteLogLine("Loading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = new GameConfiguration(General.LoadGameConfiguration(options.ConfigFile));
			configinfo.ApplyDefaults();
			General.Plugins.GameConfigurationChanged();
			
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
			General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
			io = MapSetIO.Create(config.FormatInterface, tempwad, this);
			General.WriteLogLine("Reading map data structures from file...");
			try { map = io.Read(map, TEMP_MAP_HEADER); }
			catch(Exception e)
			{
				General.WriteLogLine("ERROR: " + e.GetType().Name + ": " + e.Message);
				General.ShowErrorMessage("Unable to read the map data structures with the specified configuration.", MessageBoxButtons.OK);
				return false;
			}

			// Update structures
			map.Update();
			thingsfilter.Update();
			
			// Load data manager
			General.WriteLogLine("Loading data resources...");
			data = new DataManager();
			maplocation = new DataLocation(DataLocation.RESOURCE_WAD, filepathname, false, false);
			data.Load(configinfo.Resources, options.Resources, maplocation);
			
			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Bind any methods
			General.Actions.BindMethods(this);

			// Set default mode
			ChangeMode("VerticesMode");

			// Center map in screen
			if(General.Map.Mode is ClassicMode) (General.Map.Mode as ClassicMode).CenterInScreen();
			
			// Success
			General.WriteLogLine("Map loading done");
			return true;
		}
		
		#endregion

		#region ================== Save

		// Initializes for an existing map
		internal bool SaveMap(string newfilepathname, int savemode)
		{
			MapSet outputset;
			string nodebuildername, oldstatus, settingsfile;
			Configuration mapsettings;
			WAD targetwad;
			int index;
			bool includenodes;
			string origmapname;
			
			General.WriteLogLine("Saving map to file: " + newfilepathname);
			
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
			index = tempwad.FindLumpIndex(TEMP_MAP_HEADER);
			if(index == -1) index = 0;
			io.Write(outputset, TEMP_MAP_HEADER, index);
			
			// Get the corresponding nodebuilder
			if(savemode == SAVE_TEST) nodebuildername = configinfo.NodebuilderTest;
				else nodebuildername = configinfo.NodebuilderSave;
			
			// Build the nodes
			oldstatus = General.MainWindow.GetCurrentSatus();
			General.MainWindow.DisplayStatus("Building map nodes...");
			if((nodebuildername != null) && (nodebuildername != ""))
				includenodes = BuildNodes(nodebuildername, true);
			else
				includenodes = false;
			General.MainWindow.DisplayStatus(oldstatus);
			
			// Suspend data resources
			data.Suspend();

			try
			{
				// Except when saving INTO another file,
				// kill the target file if it is different from source file
				if((savemode != SAVE_INTO) && (newfilepathname != filepathname))
				{
					// Kill target file
					if(File.Exists(newfilepathname)) File.Delete(newfilepathname);

					// Kill .dbs settings file
					settingsfile = newfilepathname.Substring(0, newfilepathname.Length - 4) + ".dbs";
					if(File.Exists(settingsfile)) File.Delete(settingsfile);
				}

				// On Save AS we have to copy the previous file to the new file
				if((savemode == SAVE_AS) && (filepathname != ""))
				{
					// Copy if original file still exists
					if(File.Exists(filepathname)) File.Copy(filepathname, newfilepathname, true);
				}
				
				// Open the target file
				targetwad = new WAD(newfilepathname);
			}
			catch(IOException)
			{
				General.ShowErrorMessage("IO Error while writing target file: " + newfilepathname + ". Please make sure the location is accessible and not in use by another program.", MessageBoxButtons.OK);
				data.Resume();
				General.WriteLogLine("Map saving failed");
				return false;
			}
			catch(UnauthorizedAccessException)
			{
				General.ShowErrorMessage("Error while accessing target file: " + newfilepathname + ". Please make sure the location is accessible and not in use by another program.", MessageBoxButtons.OK);
				data.Resume();
				General.WriteLogLine("Map saving failed");
				return false;
			}
			
			// Determine original map name
			if(options.PreviousName != "") origmapname = options.PreviousName;
				else origmapname = options.CurrentName;

			// Copy map lumps to target file
			CopyLumpsByType(tempwad, TEMP_MAP_HEADER, targetwad, origmapname, true, true, includenodes, true);

			// Was the map lump name renamed?
			if((options.PreviousName != options.CurrentName) &&
			   (options.PreviousName != ""))
			{
				General.WriteLogLine("Renaming map lump name from " + options.PreviousName + " to " + options.CurrentName);
				
				// Find the map header in target
				index = targetwad.FindLumpIndex(options.PreviousName);
				if(index > -1)
				{
					// Rename the map lump name
					targetwad.Lumps[index].Rename(options.CurrentName);
					options.PreviousName = "";
				}
				else
				{
					// Houston, we've got a problem!
					General.ShowErrorMessage("Error renaming map lump name: the original map lump could not be found!", MessageBoxButtons.OK);
					options.CurrentName = options.PreviousName;
					options.PreviousName = "";
				}
			}

			// Done with the target file
			targetwad.Dispose();

			// Resume data resources
			data.Resume();

			try
			{
				// Open or create the map settings
				settingsfile = newfilepathname.Substring(0, newfilepathname.Length - 4) + ".dbs";
				options.WriteConfiguration(settingsfile);
			}
			catch(Exception e)
			{
				// Warning only
				General.WriteLogLine("WARNING: " + e.GetType().Name + ": " + e.Message);
				General.WriteLogLine("WARNING: Could not write the map settings configuration file!");
			}
			
			// Was the map saved in a different file? And not for testing purpose?
			if((savemode != SAVE_TEST) && (newfilepathname != filepathname))
			{
				// Keep new filename
				filepathname = newfilepathname;
				filetitle = Path.GetFileName(filepathname);
				
				// Changes saved
				changed = false;
				
				// Reload resources
				ReloadResources();
			}
			
			// Reset changed status
			if(savemode != SAVE_TEST) changed = false;
			
			// Success!
			General.WriteLogLine("Map saving done");
			return true;
		}
		
		#endregion

		#region ================== Nodebuild

		// This builds the nodes in the temproary file with the given configuration name
		private bool BuildNodes(string nodebuildername, bool failaswarning)
		{
			NodebuilderInfo nodebuilder;
			string tempfile1, tempfile2;
			bool lumpnodebuild, lumpallowempty, lumpscomplete;
			WAD buildwad;
			int srcindex;

			// Find the nodebuilder
			nodebuilder = General.GetNodebuilderByName(nodebuildername);
			if(nodebuilder == null)
			{
				// Problem! Can't find that nodebuilder!
				General.ShowWarningMessage("Unable to build the nodes: The configured nodebuilder cannot be found.\nPlease check your game configuration settings!", MessageBoxButtons.OK);
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
						foreach(DictionaryEntry ml in config.MapLumpNames)
						{
							// Read lump settings from map config
							lumpnodebuild = config.ReadSetting("maplumpnames." + ml.Key + ".nodebuild", false);
							lumpallowempty = config.ReadSetting("maplumpnames." + ml.Key + ".allowempty", false);

							// Check if this lump should exist
							if(lumpnodebuild && !lumpallowempty)
							{
								// Find the lump in the source
								if(buildwad.FindLump(ml.Key.ToString(), srcindex, srcindex + config.MapLumpNames.Count + 2) == null)
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
						// Nodebuilder did not build the lumps!
						if(failaswarning)
							General.ShowWarningMessage("Unable to build the nodes: The nodebuilder failed to build the expected data structures.\nThe map will be saved without the nodes.", MessageBoxButtons.OK);
						else
							General.ShowErrorMessage("Unable to build the nodes: The nodebuilder failed to build the expected data structures.", MessageBoxButtons.OK);
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

		#region ================== Lumps

		// This creates empty lumps for those required
		private void CreateRequiredLumps(WAD target, string mapname)
		{
			int headerindex, insertindex, targetindex;
			string lumpname;
			bool lumprequired;
			
			// Find the map header in target
			headerindex = target.FindLumpIndex(mapname);
			if(headerindex == -1)
			{
				// If this header doesnt exists in the target
				// then insert at the end of the target
				headerindex = target.Lumps.Count;
			}

			// Begin inserting at target header index
			insertindex = headerindex;

			// Go for all the map lump names
			foreach(DictionaryEntry ml in config.MapLumpNames)
			{
				// Read lump settings from map config
				lumprequired = config.ReadSetting("maplumpnames." + ml.Key + ".required", false);

				// Check if this lump is required
				if(lumprequired)
				{
					// Get the lump name
					lumpname = ml.Key.ToString();
					if(lumpname == CONFIG_MAP_HEADER) lumpname = mapname;

					// Check if the lump is missing at the target
					targetindex = FindSpecificLump(target, lumpname, headerindex, mapname, config.MapLumpNames);
					if(targetindex == -1)
					{
						// Determine target index
						insertindex++;
						if(insertindex > target.Lumps.Count) insertindex = target.Lumps.Count;

						// Create new, emtpy lump
						General.WriteLogLine(lumpname + " is required! Created empty lump.");
						target.Insert(lumpname, insertindex, 0);
					}
					else
					{
						// Move insert index
						insertindex = targetindex;
					}
				}
			}
		}
		
		// This copies specific map lumps from one WAD to another
		private void CopyLumpsByType(WAD source, string sourcemapname,
									 WAD target, string targetmapname,
									 bool copyrequired, bool copyblindcopy,
									 bool copynodebuild, bool copyscript)
		{
			bool lumprequired, lumpblindcopy, lumpnodebuild;
			string lumpscript, srclumpname, tgtlumpname;
			int srcheaderindex, tgtheaderindex, targetindex, sourceindex, lumpindex;
			Lump lump, newlump;
			
			// Find the map header in target
			tgtheaderindex = target.FindLumpIndex(targetmapname);
			if(tgtheaderindex == -1)
			{
				// If this header doesnt exists in the target
				// then insert at the end of the target
				tgtheaderindex = target.Lumps.Count;
			}

			// Begin inserting at target header index
			targetindex = tgtheaderindex;
			
			// Find the map header in source
			srcheaderindex = source.FindLumpIndex(sourcemapname);
			if(srcheaderindex > -1)
			{
				// Copy the map header from source to target
				//newlump = target.Insert(targetmapname, tgtindex++, source.Lumps[srcindex].Length);
				//source.Lumps[srcindex].CopyTo(newlump);
				
				// Go for all the map lump names
				foreach(DictionaryEntry ml in config.MapLumpNames)
				{
					// Read lump settings from map config
					lumprequired = config.ReadSetting("maplumpnames." + ml.Key + ".required", false);
					lumpblindcopy = config.ReadSetting("maplumpnames." + ml.Key + ".blindcopy", false);
					lumpnodebuild = config.ReadSetting("maplumpnames." + ml.Key + ".nodebuild", false);
					lumpscript = config.ReadSetting("maplumpnames." + ml.Key + ".script", "");

					// Check if this lump should be copied
					if((lumprequired && copyrequired) || (lumpblindcopy && copyblindcopy) ||
					   (lumpnodebuild && copynodebuild) || ((lumpscript.Length != 0) && copyscript))
					{
						// Get the lump name
						srclumpname = ml.Key.ToString();
						tgtlumpname = ml.Key.ToString();
						if(srclumpname == CONFIG_MAP_HEADER) srclumpname = sourcemapname;
						if(tgtlumpname == CONFIG_MAP_HEADER) tgtlumpname = targetmapname;
						
						// Find the lump in the source
						sourceindex = FindSpecificLump(source, srclumpname, srcheaderindex, sourcemapname, config.MapLumpNames);
						if(sourceindex > -1)
						{
							// Remove lump at target
							lumpindex = RemoveSpecificLump(target, tgtlumpname, tgtheaderindex, targetmapname, config.MapLumpNames);

							// Determine target index
							// When original lump was found and removed then insert at that position
							// otherwise insert after last insertion position
							if(lumpindex > -1) targetindex = lumpindex; else targetindex++;
							if(targetindex > target.Lumps.Count) targetindex = target.Lumps.Count;
							
							// Copy the lump to the target
							//General.WriteLogLine(srclumpname + " copying as " + tgtlumpname);
							lump = source.Lumps[sourceindex];
							newlump = target.Insert(tgtlumpname, targetindex, lump.Length);
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
		
		// This finds a lump within the range of known lump names
		// Returns -1 when the lump cannot be found
		internal static int FindSpecificLump(WAD source, string lumpname, int mapheaderindex, string mapheadername, IDictionary maplumps)
		{
			// Use the configured map lump names to find the specific lump within range,
			// because when an unknown lump is met, this search must stop.

			// Go for all lumps in order to find the specified lump
			for(int i = 0; i < maplumps.Count + 1; i++)
			{
				// Still within bounds?
				if((mapheaderindex + i) < source.Lumps.Count)
				{
					// Check if this is a known lump name
					if(maplumps.Contains(source.Lumps[mapheaderindex + i].Name) ||
					   (maplumps.Contains(CONFIG_MAP_HEADER) && (source.Lumps[mapheaderindex + i].Name == mapheadername)))
					{
						// Is this the lump we are looking for?
						if(source.Lumps[mapheaderindex + i].Name == lumpname)
						{
							// Return this index
							return mapheaderindex + i;
						}
					}
					else
					{
						// Unknown lump hit, abort search
						break;
					}
				}
			}

			// Nothing found
			return -1;
		}
		
		// This removes a specific lump and returns the position where the lump was removed
		// Returns -1 when the lump could not be found
		internal static int RemoveSpecificLump(WAD source, string lumpname, int mapheaderindex, string mapheadername, IDictionary maplumps)
		{
			int lumpindex;
			
			// Find the specific lump index
			lumpindex = FindSpecificLump(source, lumpname, mapheaderindex, mapheadername, maplumps);
			if(lumpindex > -1)
			{
				// Remove this lump
				//General.WriteLogLine(lumpname + " removed");
				source.RemoveAt(lumpindex);
			}
			else
			{
				// Lump not found
				//General.WriteLogLine("WARNING: " + lumpname + " should be removed but was not found!");
			}
			
			// Return result
			return lumpindex;
		}
		
		#endregion

		#region ================== Editing Modes
		
		/// <summary>
		/// This cancels the current mode.
		/// </summary>
		[BeginAction("cancelmode")]
		public void CancelMode()
		{
			// Let the mode know
			mode.OnCancel();
		}

		/// <summary>
		/// This accepts the changes in the current mode.
		/// </summary>
		[BeginAction("acceptmode")]
		public void AcceptMode()
		{
			// Let the mode know
			mode.OnAccept();
		}

		//
		// This changes the editing mode.
		// Order in which events occur for the old and new modes:
		// 
		// - Constructor of new mode is called
		// - Disengage of old mode is called
		// ----- Mode switches -----
		// - Engage of new mode is called
		// - Dispose of old mode is called
		//
		public void ChangeMode(EditMode nextmode)
		{
			EditMode oldmode = mode;
			cancelmodechange = false;
			
			// Log info
			if(newmode != null)
				General.WriteLogLine("Switching edit mode to " + newmode.GetType().Name + "...");
			else
				General.WriteLogLine("Stopping edit mode...");
			
			// Remember previous mode
			newmode = nextmode;
			if(mode != null)
			{
				prevmode = mode.GetType();
				if(!mode.Attributes.Volatile) prevstablemode = prevmode;
			}
			else
			{
				prevmode = null;
				prevstablemode = null;
			}
			
			// Let the plugins know beforehand
			General.Plugins.ModeChanges(oldmode, newmode);

			// Change cancelled?
			if(cancelmodechange)
			{
				General.WriteLogLine("Edit mode change cancelled.");
				return;
			}
			
			// Disenagage old mode
			if(oldmode != null) oldmode.OnDisengage();

			// Change cancelled?
			if(cancelmodechange)
			{
				General.WriteLogLine("Edit mode change cancelled.");
				return;
			}

			// Reset cursor
			General.Interface.SetCursor(Cursors.Default);
			
			// Apply new mode
			mode = newmode;
			
			// Engage new mode
			if(newmode != null) newmode.OnEngage();

			// Check appropriate button on interface
			// And show the mode name
			if(mode != null)
			{
				General.MainWindow.CheckEditModeButton(mode.EditModeButtonName);
				General.MainWindow.DisplayModeName(mode.Attributes.DisplayName);
			}
			else
			{
				General.MainWindow.CheckEditModeButton("");
				General.MainWindow.DisplayModeName("");
			}

			// Dispose old mode
			if(oldmode != null) oldmode.Dispose();

			// Done switching
			newmode = null;
			
			// Redraw the display
			General.MainWindow.RedrawDisplay();
		}
		
		// This changes mode by class name and optionally with arguments
		public void ChangeMode(string classname, params object[] args)
		{
			EditModeInfo emi = General.Plugins.GetEditModeInfo(classname);
			if(emi != null) emi.SwitchToMode(args);
		}
		
		#endregion

		#region ================== Methods
		
		// This changes thing filter
		internal void ChangeThingFilter(ThingsFilter newfilter)
		{
			// We have a special filter for null
			if(newfilter == null) newfilter = new NullThingsFilter();
			
			// Deactivate old filter
			if(thingsfilter != null) thingsfilter.Deactivate();

			// Change
			thingsfilter = newfilter;

			// Activate filter
			thingsfilter.Activate();
			
			// Redraw
			General.MainWindow.RedrawDisplay();
		}
		
		// This sets a new mapset for editing
		internal void ChangeMapSet(MapSet newmap)
		{
			// Can't have a selection in an old map set
			map.ClearAllSelected();

			// Apply
			map.Dispose();
			map = newmap;
			map.Update();
			thingsfilter.Update();
		}
		
		// This reloads resources
		[BeginAction("reloadresources")]
		internal void ReloadResources()
		{
			DataLocation maplocation;
			string oldstatus;
			Cursor oldcursor;
			
			// Keep old display info
			oldstatus = General.MainWindow.GetCurrentSatus();
			oldcursor = Cursor.Current;
			
			// Show status
			General.MainWindow.DisplayStatus("Reloading data resources...");
			Cursor.Current = Cursors.WaitCursor;
			
			// Clean up
			data.Dispose();
			data = null;
			config = null;
			configinfo = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			
			// Reload game configuration
			General.WriteLogLine("Reloading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = new GameConfiguration(General.LoadGameConfiguration(options.ConfigFile));
			General.Plugins.GameConfigurationChanged();
			
			// Reload data resources
			General.WriteLogLine("Reloading data resources...");
			data = new DataManager();
			maplocation = new DataLocation(DataLocation.RESOURCE_WAD, filepathname, false, false);
			data.Load(configinfo.Resources, options.Resources, maplocation);

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Apply new settings to map elements
			map.UpdateConfiguration();

			// Re-link the background image
			grid.LinkBackground();
			
			// Inform all plugins that the resource are reloaded
			General.Plugins.ReloadResources();
			
			// Reset status
			General.MainWindow.DisplayStatus(oldstatus);
			Cursor.Current = oldcursor;
		}

		// Game Configuration action
		[BeginAction("mapoptions")]
		internal void ShowMapOptions()
		{
			// Cancel volatile mode, if any
			General.DisengageVolatileMode();
			
			// Show map options dialog
			MapOptionsForm optionsform = new MapOptionsForm(options);
			if(optionsform.ShowDialog(General.MainWindow) == DialogResult.OK)
			{
				// Update interface
				General.MainWindow.UpdateInterface();

				// Stop data manager
				data.Dispose();
				
				// Apply new options
				this.options = optionsform.Options;

				// Load new game configuration
				General.WriteLogLine("Loading game configuration...");
				configinfo = General.GetConfigurationInfo(options.ConfigFile);
				config = new GameConfiguration(General.LoadGameConfiguration(options.ConfigFile));
				configinfo.ApplyDefaults();
				General.Plugins.GameConfigurationChanged();
				
				// Setup new map format IO
				General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
				io = MapSetIO.Create(config.FormatInterface, tempwad, this);

				// Create required lumps if they don't exist yet
				CreateRequiredLumps(tempwad, TEMP_MAP_HEADER);

				// Let the plugins know
				General.Plugins.MapReconfigure();
				
				// Update interface
				General.MainWindow.UpdateInterface();
				
				// Reload resources
				ReloadResources();
				
				// Done
				General.MainWindow.DisplayReady();
			}

			// Done
			optionsform.Dispose();
		}

		// This shows the things filters setup
		[BeginAction("thingsfilterssetup")]
		internal void ShowThingsFiltersSetup()
		{
			// Show things filter dialog
			ThingsFiltersForm f = new ThingsFiltersForm();
			f.ShowDialog(General.MainWindow);
			f.Dispose();
		}
		
		// This returns true is the given type matches
		public bool IsType(Type t)
		{
			return io.GetType().Equals(t);
		}

		#endregion
	}
}
