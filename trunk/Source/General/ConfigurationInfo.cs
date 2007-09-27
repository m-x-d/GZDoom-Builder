
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
using CodeImp.DoomBuilder.Images;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder
{
	internal class ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		#region ================== Variables

		private string name;
		private string filename;
		private string settingskey;
		private string nodebuilder;
		private bool buildonsave;
		private ResourceLocationList resources;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Filename { get { return filename; } }
		public string Nodebuilder { get { return nodebuilder; } }
		public bool BuildOnSave { get { return buildonsave; } }
		public ResourceLocationList Resources { get { return resources; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ConfigurationInfo(string name, string filename)
		{
			// Initialize
			this.name = name;
			this.filename = filename;
			this.settingskey = Path.GetFileNameWithoutExtension(filename).ToLower();
			
			// Load settings from program configuration
			this.nodebuilder = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuilder", "");
			this.buildonsave = General.Settings.ReadSetting("configurations." + settingskey + ".buildonsave", true);
			this.resources = new ResourceLocationList(General.Settings, "configurations." + settingskey + ".resources");
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
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuilder", nodebuilder);
			General.Settings.WriteSetting("configurations." + settingskey + ".buildonsave", buildonsave);
			resources.WriteToConfig(General.Settings, "configurations." + settingskey + ".resources");
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
			ci.nodebuilder = this.nodebuilder;
			ci.buildonsave = this.buildonsave;
			ci.resources = new ResourceLocationList();
			ci.resources.AddRange(this.resources);
			return ci;
		}
		
		// This applies settings from an object
		public void Apply(ConfigurationInfo ci)
		{
			this.name = ci.name;
			this.filename = ci.filename;
			this.settingskey = ci.settingskey;
			this.nodebuilder = ci.nodebuilder;
			this.buildonsave = ci.buildonsave;
			this.resources = new ResourceLocationList();
			this.resources.AddRange(ci.resources);
		}
		
		#endregion
	}
}
