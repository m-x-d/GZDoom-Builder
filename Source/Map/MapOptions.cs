
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
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	internal sealed class MapOptions
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map configuration
		private Configuration mapconfig;
		
		// Game configuration
		private string configfile;
		
		// Map header name
		private string currentname;
		private string previousname;		// When zero length string, map has not renamed
		
		// Strict pathes loading?
		private bool strictpatches;
		
		// Additional resources
		private DataLocationList resources;

		// Script files opened
		private List<string> scriptfiles;
		
		#endregion

		#region ================== Properties

		public string ConfigFile { get { return configfile; } set { configfile = value; } }
		public DataLocationList Resources { get { return resources; } }
		public bool StrictPatches { get { return strictpatches; } set { strictpatches = value; } }
		public List<string> ScriptFiles { get { return scriptfiles; } set { scriptfiles = value; } }
		public string PreviousName { get { return previousname; } set { previousname = value; } }
		public string CurrentName
		{
			get { return currentname; }

			set
			{
				// Change the name, but keep previous name
				if(currentname != value)
				{
					if(previousname == "") previousname = currentname;
					currentname = value;
				}
			}
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapOptions()
		{
			// Initialize
			this.previousname = "";
			this.currentname = "";
			this.configfile = "";
			this.strictpatches = false;
			this.resources = new DataLocationList();
			this.mapconfig = new Configuration(true);
			this.scriptfiles = new List<string>();
		}

		// Constructor to load from Doom Builder Map Settings Configuration
		internal MapOptions(Configuration cfg, string mapname)
		{
			IDictionary mapinfo, resinfo;
			DataLocation res;
			
			// Initialize
			this.previousname = "";
			this.currentname = mapname;
			this.strictpatches = General.Int2Bool(cfg.ReadSetting("strictpatches", 0));
			this.configfile = cfg.ReadSetting("gameconfig", "");
			this.resources = new DataLocationList();
			this.mapconfig = new Configuration(true);
			this.scriptfiles = new List<string>();
			
			// Read map configuration
			this.mapconfig.Root = cfg.ReadSetting("maps." + mapname, new Hashtable());

			// Resources
			IDictionary reslist = this.mapconfig.ReadSetting("resources", new Hashtable());
			foreach(DictionaryEntry mp in reslist)
			{
				// Item is a structure?
				if(mp.Value is IDictionary)
				{
					// Create resource
					resinfo = (IDictionary)mp.Value;
					res = new DataLocation();
					
					// Copy information from Configuration to ResourceLocation
					if(resinfo.Contains("type") && (resinfo["type"] is int)) res.type = (int)resinfo["type"];
					if(resinfo.Contains("location") && (resinfo["location"] is string)) res.location = (string)resinfo["location"];
					if(resinfo.Contains("textures") && (resinfo["textures"] is bool)) res.option1 = (bool)resinfo["textures"];
					if(resinfo.Contains("flats") && (resinfo["flats"] is bool)) res.option2 = (bool)resinfo["flats"];

					// Add resource
					AddResource(res);
				}
			}

			// Scripts
			IDictionary scplist = this.mapconfig.ReadSetting("scripts", new Hashtable());
			foreach(DictionaryEntry mp in scplist)
				scriptfiles.Add(mp.Value.ToString());
		}

		~MapOptions()
		{
			// Clean up
			this.resources = null;
			this.scriptfiles = null;
		}
		
		#endregion

		#region ================== Methods

		// This stores the map options in a configuration
		public void WriteConfiguration(string settingsfile)
		{
			Configuration wadcfg;
			
			// Write resources to config
			resources.WriteToConfig(mapconfig, "resources");

			// Write grid settings
			General.Map.Grid.WriteToConfig(mapconfig, "grid");

			// Write scripts to config
			mapconfig.DeleteSetting("scripts");
			for(int i = 0; i < scriptfiles.Count; i++)
				mapconfig.WriteSetting("scripts." + "file" + i.ToString(CultureInfo.InvariantCulture), scriptfiles[i]);

			// Load the file or make a new file
			if(File.Exists(settingsfile))
				wadcfg = new Configuration(settingsfile, true);
			else
				wadcfg = new Configuration(true);
			
			// Write configuration type information
			wadcfg.WriteSetting("type", "Doom Builder Map Settings Configuration");
			wadcfg.WriteSetting("gameconfig", configfile);
			wadcfg.WriteSetting("strictpatches", General.Bool2Int(strictpatches));
			
			// Update the settings file with this map configuration
			wadcfg.WriteSetting("maps." + currentname, mapconfig.Root);

			// Save file
			wadcfg.SaveConfiguration(settingsfile);
		}
		
		// This adds a resource location and returns the index where the item was added
		public int AddResource(DataLocation res)
		{
			// Get a fully qualified path
			res.location = Path.GetFullPath(res.location);
			
			// Go for all items in the list
			for(int i = 0; i < resources.Count; i++)
			{
				// Check if location is already added
				if(Path.GetFullPath(resources[i].location) == res.location)
				{
					// Update the item in the list
					resources[i] = res;
					return i;
				}
			}

			// Add to list
			resources.Add(res);
			return resources.Count - 1;
		}

		// This clears all reasource
		public void ClearResources()
		{
			// Clear list
			resources.Clear();
		}
		
		// This removes a resource by index
		public void RemoveResource(int index)
		{
			// Remove the item
			resources.RemoveAt(index);
		}
		
		// This copies resources from a list
		public void CopyResources(DataLocationList fromlist)
		{
			// Clear this list
			resources.Clear();
			resources.AddRange(fromlist);
		}

		// This loads the grid settings
		internal void ApplyGridSettings()
		{
			General.Map.Grid.ReadFromConfig(mapconfig, "grid");
		}
		
		// This displays the current map name
		public override string ToString()
		{
			return currentname;
		}
		
		// This returns the UDMF field type
		public int GetUniversalFieldType(string elementname, string fieldname, int defaulttype)
		{
			int type;
			
			// Check if the field type is set in the game configuration
			type = General.Map.Config.ReadSetting("universalfields." + elementname + "." + fieldname + ".type", -1);
			if(type == -1)
			{
				// Read from map configuration
				type = mapconfig.ReadSetting("fieldtypes." + elementname + "." + fieldname, defaulttype);
			}

			return type;
		}

		// This stores the UDMF field type
		public void SetUniversalFieldType(string elementname, string fieldname, int type)
		{
			// Check if the type of this field is not set in the game configuration
			if(General.Map.Config.ReadSetting("universalfields." + elementname + "." + fieldname + ".type", -1) == -1)
			{
				// Write type to map configuration
				mapconfig.WriteSetting("fieldtypes." + elementname + "." + fieldname, type);
			}
		}
		
		// This removes all UDMF field types
		public void ForgetUniversalFieldTypes()
		{
			mapconfig.DeleteSetting("fieldtypes");
		}
		
		#endregion
	}
}
