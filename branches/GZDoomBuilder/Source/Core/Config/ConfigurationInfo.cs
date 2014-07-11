
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using CodeImp.DoomBuilder.Editing;
using System.Collections.Specialized;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		#region ================== Constants

		private const string MODE_DISABLED_KEY = "disabled";
		private const string MODE_ENABLED_KEY = "enabled";

		// The { and } are invalid key names in a configuration so this ensures this string is unique
		private const string MISSING_NODEBUILDER = "{missing nodebuilder}";
		private readonly string[] LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR = new[] { "^" }; //mxd

		#endregion
		
		#region ================== Variables

		private string name;
		private string filename;
		private string settingskey;
		private string defaultlumpname;
		private string nodebuildersave;
		private string nodebuildertest;
		private readonly string defaultscriptcompiler; //mxd
		private DataLocationList resources;
		private Configuration config; //mxd
		private bool enabled; //mxd
		private bool changed; //mxd

		private List<EngineInfo> testEngines; //mxd
		private int currentEngineIndex; //mxd
		private LinedefColorPreset[] linedefColorPresets; //mxd

		private List<ThingsFilter> thingsfilters;
		private List<DefinedTextureSet> texturesets;
		private Dictionary<string, bool> editmodes;
		private string startmode;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Filename { get { return filename; } }
		public string DefaultLumpName { get { return defaultlumpname; } }
		public string NodebuilderSave { get { return nodebuildersave; } internal set { nodebuildersave = value; } }
		public string NodebuilderTest { get { return nodebuildertest; } internal set { nodebuildertest = value; } }
		public string DefaultScriptCompiler { get { return defaultscriptcompiler; } } //mxd
		internal DataLocationList Resources { get { return resources; } }
		internal Configuration Configuration { get { return config; } } //mxd
		public bool Enabled { get { return enabled; } internal set { enabled = value; } } //mxd
		public bool Changed { get { return changed; } internal set { changed = value; } } //mxd

		//mxd
		public string TestProgramName { get { return testEngines[currentEngineIndex].TestProgramName; } internal set { testEngines[currentEngineIndex].TestProgramName = value; } }
		public string TestProgram { get { return testEngines[currentEngineIndex].TestProgram; } internal set { testEngines[currentEngineIndex].TestProgram = value; } }
		public string TestParameters { get { return testEngines[currentEngineIndex].TestParameters; } internal set { testEngines[currentEngineIndex].TestParameters = value; } }
		public bool TestShortPaths { get { return testEngines[currentEngineIndex].TestShortPaths; } internal set { testEngines[currentEngineIndex].TestShortPaths = value; } }
		public int TestSkill { get { return testEngines[currentEngineIndex].TestSkill; } internal set { testEngines[currentEngineIndex].TestSkill = value; } }
		public bool CustomParameters { get { return testEngines[currentEngineIndex].CustomParameters; } internal set { testEngines[currentEngineIndex].CustomParameters = value; } }
		public List<EngineInfo> TestEngines { get { return testEngines; } internal set { testEngines = value; } }
		public int CurrentEngineIndex { get { return currentEngineIndex; } internal set { currentEngineIndex = value; } }
		public LinedefColorPreset[] LinedefColorPresets { get { return linedefColorPresets; } internal set { linedefColorPresets = value; } }

		internal ICollection<ThingsFilter> ThingsFilters { get { return thingsfilters; } }
		internal List<DefinedTextureSet> TextureSets { get { return texturesets; } }
		internal Dictionary<string, bool> EditModes { get { return editmodes; } }
		public string StartMode { get { return startmode; } internal set { startmode = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ConfigurationInfo(Configuration cfg, string filename)
		{
			// Initialize
			this.filename = filename;
			this.config = cfg; //mxd
			this.settingskey = Path.GetFileNameWithoutExtension(filename).ToLower();
			
			// Load settings from game configuration
			this.name = config.ReadSetting("game", "<unnamed game>");
			this.defaultlumpname = config.ReadSetting("defaultlumpname", "");
			
			// Load settings from program configuration
			this.nodebuildersave = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildersave", MISSING_NODEBUILDER);
			this.nodebuildertest = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildertest", MISSING_NODEBUILDER);
			this.defaultscriptcompiler = cfg.ReadSetting("defaultscriptcompiler", ""); //mxd
			this.resources = new DataLocationList(General.Settings.Config, "configurations." + settingskey + ".resources");
			this.startmode = General.Settings.ReadSetting("configurations." + settingskey + ".startmode", "VerticesMode");
			this.enabled = General.Settings.ReadSetting("configurations." + settingskey + ".enabled", config.ReadSetting("enabledbydefault", false)); //mxd
			
			//mxd. read test engines
			testEngines = new List<EngineInfo>();
			IDictionary list = General.Settings.ReadSetting("configurations." + settingskey + ".engines", new ListDictionary());
			currentEngineIndex = General.Settings.ReadSetting("configurations." + settingskey + ".currentengineindex", 0);
			
			//no engine list found? use old engine properties
			if (list.Count == 0) {
				EngineInfo info = new EngineInfo();
				info.TestProgram = General.Settings.ReadSetting("configurations." + settingskey + ".testprogram", "");
				info.TestProgramName = General.Settings.ReadSetting("configurations." + settingskey + ".testprogramname", EngineInfo.DEFAULT_ENGINE_NAME);
				info.CheckProgramName(false);
				info.TestParameters = General.Settings.ReadSetting("configurations." + settingskey + ".testparameters", "");
				info.TestShortPaths = General.Settings.ReadSetting("configurations." + settingskey + ".testshortpaths", false);
				info.CustomParameters = General.Settings.ReadSetting("configurations." + settingskey + ".customparameters", false);
				info.TestSkill = General.Settings.ReadSetting("configurations." + settingskey + ".testskill", 3);
				testEngines.Add(info);
				currentEngineIndex = 0;
			} else {
				//read engines settings from config
				foreach (DictionaryEntry de in list) {
					string path = "configurations." + settingskey + ".engines." + de.Key;
					EngineInfo info = new EngineInfo();
					info.TestProgram = General.Settings.ReadSetting(path + ".testprogram", "");
					info.TestProgramName = General.Settings.ReadSetting(path + ".testprogramname", EngineInfo.DEFAULT_ENGINE_NAME);
					info.CheckProgramName(false);
					info.TestParameters = General.Settings.ReadSetting(path + ".testparameters", "");
					info.TestShortPaths = General.Settings.ReadSetting(path + ".testshortpaths", false);
					info.CustomParameters = General.Settings.ReadSetting(path + ".customparameters", false);
					info.TestSkill = General.Settings.ReadSetting(path + ".testskill", 3);
					testEngines.Add(info);
				}

				if(currentEngineIndex >= testEngines.Count)	currentEngineIndex = 0;
			}

			//mxd. read custom linedef colors 
			List<LinedefColorPreset> colorPresets = new List<LinedefColorPreset>();
			list = General.Settings.ReadSetting("configurations." + settingskey + ".linedefcolorpresets", new ListDictionary());

			//no presets? add "classic" ones then.
			if(list.Count == 0) {
				LinedefColorPreset anyActionPreset = new LinedefColorPreset("Any action", PixelColor.FromColor(System.Drawing.Color.PaleGreen), -1, 0, new List<string>(), new List<string>());
				anyActionPreset.SetValid();
				colorPresets.Add(anyActionPreset);
			} else {
				//read custom linedef colors from config
				foreach(DictionaryEntry de in list) {
					string path = "configurations." + settingskey + ".linedefcolorpresets." + de.Key;
					string presetname = General.Settings.ReadSetting(path + ".name", "Unnamed");
					PixelColor color = PixelColor.FromInt(General.Settings.ReadSetting(path + ".color", -1));
					int action = General.Settings.ReadSetting(path + ".action", 0);
					int activation = General.Settings.ReadSetting(path + ".activation", 0);
					List<string> flags = new List<string>();
					flags.AddRange(General.Settings.ReadSetting(path + ".flags", "").Split(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries));
					List<string> restrictedFlags = new List<string>();
					restrictedFlags.AddRange(General.Settings.ReadSetting(path + ".restrictedflags", "").Split(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries));
					LinedefColorPreset preset = new LinedefColorPreset(presetname, color, action, activation, flags, restrictedFlags);
					colorPresets.Add(preset);
				}
			}
			linedefColorPresets = colorPresets.ToArray();

			// Make list of things filters
			thingsfilters = new List<ThingsFilter>();
			IDictionary cfgfilters = General.Settings.ReadSetting("configurations." + settingskey + ".thingsfilters", new Hashtable());
			foreach(DictionaryEntry de in cfgfilters)
			{
				thingsfilters.Add(new ThingsFilter(General.Settings.Config, "configurations." + settingskey + ".thingsfilters." + de.Key));
			}

			// Make list of texture sets
			texturesets = new List<DefinedTextureSet>();
			IDictionary sets = General.Settings.ReadSetting("configurations." + settingskey + ".texturesets", new Hashtable());
			foreach(DictionaryEntry de in sets)
			{
				texturesets.Add(new DefinedTextureSet(General.Settings.Config, "configurations." + settingskey + ".texturesets." + de.Key));
			}
			
			// Make list of edit modes
			this.editmodes = new Dictionary<string, bool>(StringComparer.Ordinal);
			IDictionary modes = General.Settings.ReadSetting("configurations." + settingskey + ".editmodes", new Hashtable());
			foreach(DictionaryEntry de in modes)
			{
				if(de.Key.ToString().StartsWith(MODE_ENABLED_KEY))
					editmodes.Add(de.Value.ToString(), true);
				else if(de.Key.ToString().StartsWith(MODE_DISABLED_KEY))
					editmodes.Add(de.Value.ToString(), false);
			}
		}
		
		// Constructor
		private ConfigurationInfo()
		{
		}
		
		#endregion

		#region ================== Methods

		/// <summary>
		/// This returns the resource locations as configured.
		/// </summary>
		public DataLocationList GetResources()
		{
			return new DataLocationList(resources);
		}

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(ConfigurationInfo other)
		{
			// Compare
			return name.CompareTo(other.name);
		}

		// This saves the settings to program configuration
		internal void SaveSettings()
		{
			//mxd
			General.Settings.WriteSetting("configurations." + settingskey + ".enabled", enabled);
			if(!changed) return;
			
			// Write to configuration
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildersave", nodebuildersave);
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildertest", nodebuildertest);
			
			//mxd. Test Engines
			General.Settings.WriteSetting("configurations." + settingskey + ".currentengineindex", currentEngineIndex);
			for (int i = 0; i < testEngines.Count; i++ ) {
				string path = "configurations." + settingskey + ".engines.engine" + i.ToString(CultureInfo.InvariantCulture);
				General.Settings.WriteSetting(path + ".testprogramname", testEngines[i].TestProgramName);
				General.Settings.WriteSetting(path + ".testprogram", testEngines[i].TestProgram);
				General.Settings.WriteSetting(path + ".testparameters", testEngines[i].TestParameters);
				General.Settings.WriteSetting(path + ".testshortpaths", testEngines[i].TestShortPaths);
				General.Settings.WriteSetting(path + ".customparameters", testEngines[i].CustomParameters);
				General.Settings.WriteSetting(path + ".testskill", testEngines[i].TestSkill);
			}

			//mxd. Custom linedef colors
			for(int i = 0; i < linedefColorPresets.Length; i++) {
				string path = "configurations." + settingskey + ".linedefcolorpresets.preset" + i.ToString(CultureInfo.InvariantCulture);
				General.Settings.WriteSetting(path + ".name", linedefColorPresets[i].Name);
				General.Settings.WriteSetting(path + ".color", linedefColorPresets[i].Color.ToInt());
				General.Settings.WriteSetting(path + ".action", linedefColorPresets[i].Action);
				General.Settings.WriteSetting(path + ".activation", linedefColorPresets[i].Activation);
				General.Settings.WriteSetting(path + ".flags", string.Join(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR[0], linedefColorPresets[i].Flags.ToArray()));
				General.Settings.WriteSetting(path + ".restrictedflags", string.Join(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR[0], linedefColorPresets[i].RestrictedFlags.ToArray()));
			}

			General.Settings.WriteSetting("configurations." + settingskey + ".startmode", startmode);
			resources.WriteToConfig(General.Settings.Config, "configurations." + settingskey + ".resources");
			
			// Write filters to configuration
			General.Settings.DeleteSetting("configurations." + settingskey + ".thingsfilters");
			for(int i = 0; i < thingsfilters.Count; i++)
			{
				thingsfilters[i].WriteSettings(General.Settings.Config,
					"configurations." + settingskey + ".thingsfilters.filter" + i.ToString(CultureInfo.InvariantCulture));
			}

			// Write texturesets to configuration
			for(int i = 0; i < texturesets.Count; i++)
			{
				texturesets[i].WriteToConfig(General.Settings.Config,
					"configurations." + settingskey + ".texturesets.set" + i.ToString(CultureInfo.InvariantCulture));
			}
			
			// Write filters to configuration
			ListDictionary modeslist = new ListDictionary();
			int index = 0;
			foreach(KeyValuePair<string, bool> em in editmodes)
			{
				if(em.Value)
					modeslist.Add(MODE_ENABLED_KEY + index.ToString(CultureInfo.InvariantCulture), em.Key);
				else
					modeslist.Add(MODE_DISABLED_KEY + index.ToString(CultureInfo.InvariantCulture), em.Key);

				index++;
			}
			General.Settings.WriteSetting("configurations." + settingskey + ".editmodes", modeslist);
		}

		// String representation
		public override string ToString()
		{
			return name;
		}

		// This clones the object
		internal ConfigurationInfo Clone()
		{
			ConfigurationInfo ci = new ConfigurationInfo();
			ci.name = this.name;
			ci.filename = this.filename;
			ci.settingskey = this.settingskey;
			ci.nodebuildersave = this.nodebuildersave;
			ci.nodebuildertest = this.nodebuildertest;
			ci.resources = new DataLocationList();
			ci.resources.AddRange(this.resources);
			
			//mxd
			ci.TestEngines = new List<EngineInfo>();
			foreach (EngineInfo info in testEngines) ci.TestEngines.Add(new EngineInfo(info));
			ci.LinedefColorPresets = new LinedefColorPreset[linedefColorPresets.Length];
			for(int i = 0; i < linedefColorPresets.Length; i++)
				ci.LinedefColorPresets[i] = new LinedefColorPreset(linedefColorPresets[i]);

			ci.startmode = this.startmode;
			ci.config = this.config; //mxd
			ci.enabled = this.enabled; //mxd
			ci.changed = this.changed; //mxd
			ci.texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in this.texturesets) ci.texturesets.Add(s.Copy());
			ci.thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in this.thingsfilters) ci.thingsfilters.Add(new ThingsFilter(f));
			ci.editmodes = new Dictionary<string, bool>(this.editmodes);
			return ci;
		}
		
		// This applies settings from an object
		internal void Apply(ConfigurationInfo ci)
		{
			this.name = ci.name;
			this.filename = ci.filename;
			this.settingskey = ci.settingskey;
			this.nodebuildersave = ci.nodebuildersave;
			this.nodebuildertest = ci.nodebuildertest;
			this.resources = new DataLocationList();
			this.resources.AddRange(ci.resources);
			
			//mxd
			this.testEngines = new List<EngineInfo>();
			foreach (EngineInfo info in ci.TestEngines) testEngines.Add(new EngineInfo(info));
			if (this.CurrentEngineIndex >= testEngines.Count) this.CurrentEngineIndex = testEngines.Count - 1;
			this.linedefColorPresets = new LinedefColorPreset[ci.linedefColorPresets.Length];
			for(int i = 0; i < ci.linedefColorPresets.Length; i++)
				this.linedefColorPresets[i] = new LinedefColorPreset(ci.linedefColorPresets[i]);

			this.startmode = ci.startmode;
			this.config = ci.config; //mxd
			this.enabled = ci.enabled; //mxd
			this.changed = ci.changed;
			this.texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in ci.texturesets) this.texturesets.Add(s.Copy());
			this.thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in ci.thingsfilters) this.thingsfilters.Add(new ThingsFilter(f));
			this.editmodes = new Dictionary<string, bool>(ci.editmodes);
		}
		
		// This applies the defaults
		internal void ApplyDefaults(GameConfiguration gameconfig)
		{
			// Some of the defaults can only be applied from game configuration
			if(gameconfig != null)
			{
				// No nodebuildes set?
				if(nodebuildersave == MISSING_NODEBUILDER) nodebuildersave = gameconfig.DefaultSaveCompiler;
				if(nodebuildertest == MISSING_NODEBUILDER) nodebuildertest = gameconfig.DefaultTestCompiler;
				
				// No texture sets?
				if(texturesets.Count == 0)
				{
					// Copy the default texture sets from the game configuration
					foreach(DefinedTextureSet s in gameconfig.TextureSets)
					{
						// Add a copy to our list
						texturesets.Add(s.Copy());
					}
				}
				
				// No things filters?
				if(thingsfilters.Count == 0)
				{
					// Copy the things filters from game configuration
					foreach(ThingsFilter f in gameconfig.ThingsFilters)
					{
						thingsfilters.Add(new ThingsFilter(f));
					}
				}

				//mxd. Validate filters
				foreach(ThingsFilter f in thingsfilters) f.Validate();
			}
			
			// Go for all available editing modes
			foreach(EditModeInfo info in General.Editing.ModesInfo)
			{
				// Is this a mode that is optional?
				if(info.IsOptional)
				{
					// Add if not listed yet
					if(!editmodes.ContainsKey(info.Type.FullName))
						editmodes.Add(info.Type.FullName, info.Attributes.UseByDefault);
				}
			}
		}
		
		#endregion
	}
}
