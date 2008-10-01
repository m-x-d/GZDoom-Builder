
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
using CodeImp.DoomBuilder.Data;
using System.IO;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	internal class ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		#region ================== Variables

		private string name;
		private string filename;
		private string settingskey;
		private string defaultlumpname;
		private string nodebuildersave;
		private string nodebuildertest;
		private DataLocationList resources;
		private string testprogram;
		private string testparameters;
		private bool customparameters;
		private int testskill;
		private List<ThingsFilter> thingsfilters;
		private List<TextureSet> texturesets;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Filename { get { return filename; } }
		public string DefaultLumpName { get { return defaultlumpname; } }
		public string NodebuilderSave { get { return nodebuildersave; } set { nodebuildersave = value; } }
		public string NodebuilderTest { get { return nodebuildertest; } set { nodebuildertest = value; } }
		public DataLocationList Resources { get { return resources; } }
		public string TestProgram { get { return testprogram; } set { testprogram = value; } }
		public string TestParameters { get { return testparameters; } set { testparameters = value; } }
		public int TestSkill { get { return testskill; } set { testskill = value; } }
		public bool CustomParameters { get { return customparameters; } set { customparameters = value; } }
		internal ICollection<ThingsFilter> ThingsFilters { get { return thingsfilters; } }
		public List<TextureSet> TextureSets { get { return texturesets; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ConfigurationInfo(Configuration cfg, string filename)
		{
			// Initialize
			this.filename = filename;
			this.settingskey = Path.GetFileNameWithoutExtension(filename).ToLower();
			
			// Load settings from game configuration
			this.name = cfg.ReadSetting("game", "<unnamed game>");
			this.defaultlumpname = cfg.ReadSetting("defaultlumpname", "");
			
			// Load settings from program configuration
			this.nodebuildersave = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildersave", "");
			this.nodebuildertest = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildertest", "");
			this.testprogram = General.Settings.ReadSetting("configurations." + settingskey + ".testprogram", "");
			this.testparameters = General.Settings.ReadSetting("configurations." + settingskey + ".testparameters", "");
			this.customparameters = General.Settings.ReadSetting("configurations." + settingskey + ".customparameters", false);
			this.testskill = General.Settings.ReadSetting("configurations." + settingskey + ".testskill", 3);
			this.resources = new DataLocationList(General.Settings.Config, "configurations." + settingskey + ".resources");
			
			// Make list of things filters
			thingsfilters = new List<ThingsFilter>();
			IDictionary cfgfilters = General.Settings.ReadSetting("configurations." + settingskey + ".thingsfilters", new Hashtable());
			foreach(DictionaryEntry de in cfgfilters)
			{
				thingsfilters.Add(new ThingsFilter(General.Settings.Config, "configurations." + settingskey + ".thingsfilters." + de.Key));
			}

			// Make list of texture sets
			texturesets = new List<TextureSet>();
			IDictionary sets = General.Settings.ReadSetting("configurations." + settingskey + ".texturesets", new Hashtable());
			foreach(DictionaryEntry de in sets)
			{
				texturesets.Add(new DefinedTextureSet(General.Settings.Config, "configurations." + settingskey + ".texturesets." + de.Key));
			}
		}

		// Constructor
		private ConfigurationInfo()
		{
		}
		
		#endregion

		#region ================== Methods

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(ConfigurationInfo other)
		{
			// Compare
			return name.CompareTo(other.name);
		}

		// This saves the settings to program configuration
		public void SaveSettings()
		{
			// Write to configuration
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildersave", nodebuildersave);
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildertest", nodebuildertest);
			General.Settings.WriteSetting("configurations." + settingskey + ".testprogram", testprogram);
			General.Settings.WriteSetting("configurations." + settingskey + ".testparameters", testparameters);
			General.Settings.WriteSetting("configurations." + settingskey + ".customparameters", customparameters);
			General.Settings.WriteSetting("configurations." + settingskey + ".testskill", testskill);
			resources.WriteToConfig(General.Settings.Config, "configurations." + settingskey + ".resources");

			// Write filters to configuration
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
		}

		// String representation
		public override string ToString()
		{
			return name;
		}

		// This clones the object
		public ConfigurationInfo Clone()
		{
			ConfigurationInfo ci = new ConfigurationInfo();
			ci.name = this.name;
			ci.filename = this.filename;
			ci.settingskey = this.settingskey;
			ci.nodebuildersave = this.nodebuildersave;
			ci.nodebuildertest = this.nodebuildertest;
			ci.resources = new DataLocationList();
			ci.resources.AddRange(this.resources);
			ci.testprogram = this.testprogram;
			ci.testparameters = this.testparameters;
			ci.customparameters = this.customparameters;
			ci.testskill = this.testskill;
			ci.texturesets = new List<TextureSet>();
			foreach(TextureSet s in this.texturesets) ci.texturesets.Add(s.Copy());
			return ci;
		}
		
		// This applies settings from an object
		public void Apply(ConfigurationInfo ci)
		{
			this.name = ci.name;
			this.filename = ci.filename;
			this.settingskey = ci.settingskey;
			this.nodebuildersave = ci.nodebuildersave;
			this.nodebuildertest = ci.nodebuildertest;
			this.resources = new DataLocationList();
			this.resources.AddRange(ci.resources);
			this.testprogram = ci.testprogram;
			this.testparameters = ci.testparameters;
			this.customparameters = ci.customparameters;
			this.testskill = ci.testskill;
			this.texturesets = new List<TextureSet>();
			foreach(TextureSet s in ci.texturesets) this.texturesets.Add(s.Copy());
		}
		
		// This applies the defaults
		public void ApplyDefaults()
		{
			// No texture sets?
			if(texturesets.Count == 0)
			{
				// Copy the default texture sets from the game configuration
				foreach(TextureSet s in General.Map.Config.TextureSets)
				{
					// Add a copy to our list
					texturesets.Add(s.Copy());
				}
			}
		}
		
		#endregion
	}
}
