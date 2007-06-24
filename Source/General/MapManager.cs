
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
		private MapSet data;
		private MapOptions options;
		private Configuration config;
		private EditMode mode;
		private Graphics graphics;
		private WAD tempwad;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string FilePathName { get { return filepathname; } }
		public string FileTitle { get { return filetitle; } }
		public MapOptions Options { get { return options; } }
		public MapSet Data { get { return data; } }
		public EditMode Mode { get { return mode; } }
		public bool IsChanged { get { return changed; } set { changed = value; } }
		public bool IsDisposed { get { return isdisposed; } }
		public Graphics Graphics { get { return graphics; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new map
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
				tempwad.Dispose();
				data.Dispose();
				mode.Dispose();
				graphics.Dispose();

				// Remove temp file
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
			// Apply settings
			this.filetitle = "unnamed.wad";
			this.filepathname = "";
			this.changed = false;
			this.options = options;
			
			// Create objects
			graphics = new Graphics(General.MainWindow.Display);
			config = General.LoadGameConfiguration(options.ConfigFile);
			data = new MapSet();

			// Initiate graphics
			if(!graphics.Initialize()) return false;
			
			// Set default mode
			ChangeMode(typeof(FrozenOverviewMode));

			// Success
			return true;
		}

		// Initializes for an existing map
		public bool InitializeOpenMap(string filepathname, MapOptions options)
		{
			WAD mapwad;
			MapSetIO mapio;
			
			// Apply settings
			this.filetitle = Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.changed = false;
			this.options = options;

			// Create objects
			graphics = new Graphics(General.MainWindow.Display);
			config = General.LoadGameConfiguration(options.ConfigFile);
			data = new MapSet();

			// Create temp wadfile
			tempwad = new WAD(General.MakeTempFilename());

			// Now open the map file
			mapwad = new WAD(filepathname, true);

			// Copy the map lumps to the temp file
			CopyLumpsByType(mapwad, options.CurrentName, tempwad, TEMP_MAP_HEADER,
							true, true, true, true);

			// Close the map file
			mapwad.Dispose();
			
			// Read the map from temp file
			mapio = MapSetIO.Create(config.ReadSetting("formatinterface", ""), tempwad);
			data = mapio.Read(new MapSet(), TEMP_MAP_HEADER);
			
			// Initiate graphics
			if(!graphics.Initialize()) return false;

			// Set default mode
			ChangeMode(typeof(FrozenOverviewMode));

			// Success
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
							newlump = target.Insert(ml.Key.ToString(), tgtindex++, lump.Length);
							lump.CopyTo(newlump);
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

					// Check if this lump will be copied from source
					if((lumprequired && copyrequired) || (lumpblindcopy && copyblindcopy) ||
					   (lumpnodebuild && copynodebuild) || ((lumpscript != "") && copyscript))
					{
						// Then remove it from target
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
			mode = EditMode.Create(modetype, args);
			
			// Redraw the display
			mode.RedrawDisplay();
		}
		
		#endregion
	}
}
