
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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Plugins;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class MapOptions
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

		// mxd. Script compiler
		private string scriptcompiler;

		//mxd. Sector drawing options
		private string defaultfloortexture;
		private string defaultceiltexture;
		private string defaulttoptexture;
		private string defaultwalltexture;
		private string defaultbottomtexture;
		private int custombrightness;
		private int customfloorheight;
		private int customceilheight;

		//mxd. Sector drawing overrides
		private bool overridefloortexture;
		private bool overrideceiltexture;
		private bool overridetoptexture;
		private bool overridemiddletexture;
		private bool overridebottomtexture;
		private bool overridefloorheight;
		private bool overrideceilheight;
		private bool overridebrightness;

		//mxd.
		private bool uselongtexturenames;

		//mxd. Position and scale
		private Vector2D viewposition;
		private float viewscale;
		
		#endregion

		#region ================== Properties

		internal string ConfigFile { get { return configfile; } set { configfile = value; } }
		internal DataLocationList Resources { get { return resources; } }
		internal bool StrictPatches { get { return strictpatches; } set { strictpatches = value; } }
		internal List<string> ScriptFiles { get { return scriptfiles; } set { scriptfiles = value; } }
		internal string ScriptCompiler { get { return scriptcompiler; } set { scriptcompiler = value; } } //mxd
		internal string PreviousName { get { return previousname; } set { previousname = value; } }
		internal string CurrentName
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
		internal bool LevelNameChanged { get { return (!string.IsNullOrEmpty(previousname) && previousname != currentname); } } //mxd

		public string LevelName { get { return currentname; } }
		public Dictionary<int, string> TagLabels { get { return tagLabels; } internal set { tagLabels = value; } } //mxd 
		private Dictionary<int, string> tagLabels;

		//mxd. Sector drawing options
		public string DefaultTopTexture { get { return defaulttoptexture; } set { defaulttoptexture = value; } }
		public string DefaultWallTexture { get { return defaultwalltexture; } set { defaultwalltexture = value; } }
		public string DefaultBottomTexture { get { return defaultbottomtexture; } set { defaultbottomtexture = value; } }
		public string DefaultFloorTexture { get { return defaultfloortexture; } set { defaultfloortexture = value; } }
		public string DefaultCeilingTexture { get { return defaultceiltexture; } set { defaultceiltexture = value; } }
		public int CustomBrightness { get { return custombrightness; } set { custombrightness = value; } }
		public int CustomFloorHeight { get { return customfloorheight; } set { customfloorheight = value; } }
		public int CustomCeilingHeight { get { return customceilheight; } set { customceilheight = value; } }

		//mxd. Sector drawing overrides
		public bool OverrideFloorTexture { get { return overridefloortexture; } set { overridefloortexture = value; } }
		public bool OverrideCeilingTexture { get { return overrideceiltexture; } set { overrideceiltexture = value; } }
		public bool OverrideTopTexture { get { return overridetoptexture; } set { overridetoptexture = value; } }
		public bool OverrideMiddleTexture { get { return overridemiddletexture; } set { overridemiddletexture = value; } }
		public bool OverrideBottomTexture { get { return overridebottomtexture; } set { overridebottomtexture = value; } }
		public bool OverrideFloorHeight { get { return overridefloorheight; } set { overridefloorheight = value; } }
		public bool OverrideCeilingHeight { get { return overrideceilheight; } set { overrideceilheight = value; } }
		public bool OverrideBrightness { get { return overridebrightness; } set { overridebrightness = value; } }

		//mxd
		public bool UseLongTextureNames { get { return uselongtexturenames; } set { uselongtexturenames = value; } }

		//mxd. Position and scale
		public Vector2D ViewPosition { get { return viewposition; } }
		public float ViewScale { get { return viewscale; } }

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
			this.scriptcompiler = ""; //mxd
			this.tagLabels = new Dictionary<int, string>(); //mxd
			this.viewposition = new Vector2D(float.NaN, float.NaN); //mxd
			this.viewscale = float.NaN; //mxd

			//mxd. Sector drawing options
			this.custombrightness = 196;
			this.customceilheight = 128;
		}

		// Constructor to load from Doom Builder Map Settings Configuration
		internal MapOptions(Configuration cfg, string mapname, bool longtexturenamessupported)
		{
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

			//mxd. Read Tag Labels
			this.tagLabels = new Dictionary<int, string>();
			ListDictionary tagLabelsData = (ListDictionary)this.mapconfig.ReadSetting("taglabels", new ListDictionary());

			foreach(DictionaryEntry tagLabelsEntry in tagLabelsData) 
			{
				int tag = 0;
				string label = string.Empty;

				foreach(DictionaryEntry entry in (ListDictionary)tagLabelsEntry.Value) 
				{
					switch((string)entry.Key)
					{
						case "tag": tag = (int)entry.Value; break;
						case "label": label = (string)entry.Value; break;
					}
				}

				if(tag != 0 && !string.IsNullOrEmpty(label))
					tagLabels.Add(tag, label);
			}

			//mxd. Script compiler
			scriptcompiler = this.mapconfig.ReadSetting("scriptcompiler", string.Empty);

			//mxd. Read Sector drawing options
			defaultfloortexture = this.mapconfig.ReadSetting("defaultfloortexture", string.Empty);
			defaultceiltexture = this.mapconfig.ReadSetting("defaultceiltexture", string.Empty);
			defaulttoptexture = this.mapconfig.ReadSetting("defaulttoptexture", string.Empty);
			defaultwalltexture = this.mapconfig.ReadSetting("defaultwalltexture", string.Empty);
			defaultbottomtexture = this.mapconfig.ReadSetting("defaultbottomtexture", string.Empty);
			custombrightness = General.Clamp(this.mapconfig.ReadSetting("custombrightness", 196), 0, 255);
			customfloorheight = this.mapconfig.ReadSetting("customfloorheight", 0);
			customceilheight = this.mapconfig.ReadSetting("customceilheight", 128);

			//mxd. Read Sector drawing overrides
			overridefloortexture = this.mapconfig.ReadSetting("overridefloortexture", false);
			overrideceiltexture = this.mapconfig.ReadSetting("overrideceiltexture", false);
			overridetoptexture = this.mapconfig.ReadSetting("overridetoptexture", false);
			overridemiddletexture = this.mapconfig.ReadSetting("overridemiddletexture", false);
			overridebottomtexture = this.mapconfig.ReadSetting("overridebottomtexture", false);
			overridefloorheight = this.mapconfig.ReadSetting("overridefloorheight", false);
			overrideceilheight = this.mapconfig.ReadSetting("overrideceilheight", false);
			overridebrightness = this.mapconfig.ReadSetting("overridebrightness", false);

			//mxd
			uselongtexturenames = longtexturenamessupported && this.mapconfig.ReadSetting("uselongtexturenames", false);

			//mxd. Position and scale
			float vpx = this.mapconfig.ReadSetting("viewpositionx", float.NaN);
			float vpy = this.mapconfig.ReadSetting("viewpositiony", float.NaN);
			if(!float.IsNaN(vpx) && !float.IsNaN(vpy)) viewposition = new Vector2D(vpx, vpy);
			viewscale = this.mapconfig.ReadSetting("viewscale", float.NaN);

			// Resources
			IDictionary reslist = this.mapconfig.ReadSetting("resources", new Hashtable());
			foreach(DictionaryEntry mp in reslist)
			{
				// Item is a structure?
				if(mp.Value is IDictionary)
				{
					// Create resource
					IDictionary resinfo = (IDictionary)mp.Value;
					DataLocation res = new DataLocation();
					
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

		// This makes the path prefix for the given assembly
		private static string GetPluginPathPrefix(Assembly asm)
		{
			Plugin p = General.Plugins.FindPluginByAssembly(asm);
			return "plugins." + p.Name.ToLowerInvariant() + ".";
		}

		// ReadPluginSetting
		public string ReadPluginSetting(string setting, string defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public int ReadPluginSetting(string setting, int defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public float ReadPluginSetting(string setting, float defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public short ReadPluginSetting(string setting, short defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public long ReadPluginSetting(string setting, long defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public bool ReadPluginSetting(string setting, bool defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public byte ReadPluginSetting(string setting, byte defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public IDictionary ReadPluginSetting(string setting, IDictionary defaultsetting) { return mapconfig.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }

		// ReadPluginSetting with specific plugin
		public string ReadPluginSetting(string pluginname, string setting, string defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public int ReadPluginSetting(string pluginname, string setting, int defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public float ReadPluginSetting(string pluginname, string setting, float defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public short ReadPluginSetting(string pluginname, string setting, short defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public long ReadPluginSetting(string pluginname, string setting, long defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public bool ReadPluginSetting(string pluginname, string setting, bool defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public byte ReadPluginSetting(string pluginname, string setting, byte defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public IDictionary ReadPluginSetting(string pluginname, string setting, IDictionary defaultsetting) { return mapconfig.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }

		// WritePluginSetting
		public bool WritePluginSetting(string setting, object settingvalue) { return mapconfig.WriteSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, settingvalue); }

		// DeletePluginSetting
		public bool DeletePluginSetting(string setting) { return mapconfig.DeleteSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting); }
		
		// This stores the map options in a configuration
		internal void WriteConfiguration(string settingsfile)
		{
			Configuration wadcfg;
			
			// Write resources to config
			resources.WriteToConfig(mapconfig, "resources");

			//mxd. Save selection groups
			General.Map.Map.WriteSelectionGroups(mapconfig);

			//mxd. Save Tag Labels
			if(tagLabels.Count > 0) 
			{
				ListDictionary tagLabelsData = new ListDictionary();
				int counter = 1;

				foreach(KeyValuePair<int, string> group in tagLabels)
				{
					ListDictionary data = new ListDictionary();
					data.Add("tag", group.Key);
					data.Add("label", group.Value);
					tagLabelsData.Add("taglabel"+counter, data);
					counter++;
				}

				mapconfig.WriteSetting("taglabels", tagLabelsData);
			}

			//mxd. Write Sector drawing options
			mapconfig.WriteSetting("defaultfloortexture", defaultfloortexture);
			mapconfig.WriteSetting("defaultceiltexture", defaultceiltexture);
			mapconfig.WriteSetting("defaulttoptexture", defaulttoptexture);
			mapconfig.WriteSetting("defaultwalltexture", defaultwalltexture);
			mapconfig.WriteSetting("defaultbottomtexture", defaultbottomtexture);
			mapconfig.WriteSetting("custombrightness", custombrightness);
			mapconfig.WriteSetting("customfloorheight", customfloorheight);
			mapconfig.WriteSetting("customceilheight", customceilheight);

			//mxd. Write Sector drawing overrides
			mapconfig.WriteSetting("overridefloortexture", overridefloortexture);
			mapconfig.WriteSetting("overrideceiltexture", overrideceiltexture);
			mapconfig.WriteSetting("overridetoptexture", overridetoptexture);
			mapconfig.WriteSetting("overridemiddletexture", overridemiddletexture);
			mapconfig.WriteSetting("overridebottomtexture", overridebottomtexture);
			mapconfig.WriteSetting("overridefloorheight", overridefloorheight);
			mapconfig.WriteSetting("overrideceilheight", overrideceilheight);
			mapconfig.WriteSetting("overridebrightness", overridebrightness);

			//mxd
			mapconfig.WriteSetting("uselongtexturenames", uselongtexturenames);

			//mxd. Position and scale
			mapconfig.WriteSetting("viewpositionx", General.Map.Renderer2D.OffsetX);
			mapconfig.WriteSetting("viewpositiony", General.Map.Renderer2D.OffsetY);
			mapconfig.WriteSetting("viewscale", General.Map.Renderer2D.Scale);

			//mxd. Write script compiler
			if(!string.IsNullOrEmpty(scriptcompiler))
				mapconfig.WriteSetting("scriptcompiler", scriptcompiler);

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
		internal int AddResource(DataLocation res)
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

		/// <summary>
		/// This returns the resource locations as configured.
		/// </summary>
		public DataLocationList GetResources()
		{
			return new DataLocationList(resources);
		}

		// This clears all reasource
		internal void ClearResources()
		{
			// Clear list
			resources.Clear();
		}
		
		// This removes a resource by index
		internal void RemoveResource(int index)
		{
			// Remove the item
			resources.RemoveAt(index);
		}
		
		// This copies resources from a list
		internal void CopyResources(DataLocationList fromlist)
		{
			// Clear this list
			resources.Clear();
			resources.AddRange(fromlist);
		}

		// This loads the grid settings
		internal void ApplyGridSettings()
		{
			General.Map.Grid.ReadFromConfig(mapconfig, "grid", General.Map.Config.UseLongTextureNames);
		}

		//mxd. This reads stored selection groups from the map configuration
		internal void ReadSelectionGroups() 
		{
			General.Map.Map.ReadSelectionGroups(mapconfig);
		}
		
		// This displays the current map name
		public override string ToString()
		{
			return currentname;
		}
		
		// This returns the UDMF field type
		internal int GetUniversalFieldType(string elementname, string fieldname, int defaulttype)
		{
			// Check if the field type is set in the game configuration
			int type = General.Map.Config.ReadSetting("universalfields." + elementname + "." + fieldname + ".type", -1);
			if(type == -1)
			{
				// Read from map configuration
				type = mapconfig.ReadSetting("fieldtypes." + elementname + "." + fieldname, defaulttype);
			}

			return type;
		}

		// This stores the UDMF field type
		internal void SetUniversalFieldType(string elementname, string fieldname, int type)
		{
			// Check if the type of this field is not set in the game configuration
			if(General.Map.Config.ReadSetting("universalfields." + elementname + "." + fieldname + ".type", -1) == -1)
			{
				// Write type to map configuration
				mapconfig.WriteSetting("fieldtypes." + elementname + "." + fieldname, type);
			}
		}
		
		// This removes all UDMF field types
		internal void ForgetUniversalFieldTypes()
		{
			mapconfig.DeleteSetting("fieldtypes");
		}
		
		#endregion
	}
}
