
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

		private const string TEMP_MAP_HEADER = "TEMPMAP";

		#endregion

		#region ================== Variables

		// Status
		private bool changed;
		
		// Map information
		private string filetitle;
		private string filepathname;
		private MapSet map;
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
		public MapOptions Options { get { return options; } }
		public MapSet Map { get { return map; } }
		public EditMode Mode { get { return mode; } }
		public bool IsChanged { get { return changed; } set { changed = value; } }
		public bool IsDisposed { get { return isdisposed; } }
		public D3DGraphics Graphics { get { return graphics; } }
		
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
				General.WriteLogLine("Removing temporary file...");
				try { File.Delete(tempwad.Filename); } catch(Exception) { }

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
			
			// Apply settings
			this.filetitle = "unnamed.wad";
			this.filepathname = "";
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Creating new map '" + options.CurrentName + "' with configuration '" + options.ConfigFile + "'");

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
			tempfile = General.MakeTempFilename();
			General.WriteLogLine("Creating temporary file: " + tempfile);
			tempwad = new WAD(tempfile);

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
			MapSetIO mapio;
			string tempfile;
			string iointerface;
			DataLocation maplocation;
			
			// Apply settings
			this.filetitle = Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Opening map '" + options.CurrentName + "' with configuration '" + options.ConfigFile + "'");
			
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
			tempfile = General.MakeTempFilename();
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
			mapio = MapSetIO.Create(iointerface, tempwad);
			General.WriteLogLine("Reading map data...");
			map = mapio.Read(map, TEMP_MAP_HEADER);

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
			
			// If this header doesnt exists in the target
			// then insert at the end of the target
			tgtindex = target.Lumps.Count;

			// Remove the lumps from target
			RemoveLumpsByType(target, targetmapname, copyrequired,
						copyblindcopy, copynodebuild, copyscript);

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
							General.WriteLogLine(ml.Key.ToString() + " copying");
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
									   bool copyrequired, bool copyblindcopy,
									   bool copynodebuild, bool copyscript)
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
					if((lumprequired && copyrequired) || (lumpblindcopy && copyblindcopy) ||
					   (lumpnodebuild && copynodebuild) || ((lumpscript != "") && copyscript))
					{
						// Then remove it from target
						General.WriteLogLine(nextlumpname + " removing");
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
