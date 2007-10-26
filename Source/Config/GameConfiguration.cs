
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
using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class GameConfiguration
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Original configuration
		private Configuration cfg;
		
		// General settings
		private float defaulttexturescale;
		private float defaultflatscale;
		private string formatinterface;
		private int soundlinedefflags;
		
		// Map lumps
		private IDictionary maplumpnames;
		
		// Texture/flat sources
		private IDictionary textureranges;
		private IDictionary flatranges;
		
		// Things
		private List<ThingCategory> thingcategories;
		private Dictionary<int, ThingTypeInfo> things;
		
		#endregion

		#region ================== Properties

		// General settings
		public float DefaultTextureScale { get { return defaulttexturescale; } }
		public float DefaultFlatScale { get { return defaultflatscale; } }
		public string FormatInterface { get { return formatinterface; } }
		public int SoundLinedefFlags { get { return soundlinedefflags; } }
		
		// Map lumps
		public IDictionary MapLumpNames { get { return maplumpnames; } }

		// Texture/flat sources
		public IDictionary TextureRanges { get { return textureranges; } }
		public IDictionary FlatRanges { get { return flatranges; } }

		// Things
		public List<ThingCategory> ThingCategories { get { return thingcategories; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public GameConfiguration(Configuration cfg)
		{
			IDictionary dic;
			ThingCategory thingcat;
			
			// Initialize
			this.cfg = cfg;
			this.thingcategories = new List<ThingCategory>();
			this.things = new Dictionary<int, ThingTypeInfo>();
			
			// Read general settings
			defaulttexturescale = cfg.ReadSetting("defaulttexturescale", 1f);
			defaultflatscale = cfg.ReadSetting("defaultflatscale", 1f);
			formatinterface = cfg.ReadSetting("formatinterface", "");
			soundlinedefflags = cfg.ReadSetting("soundlinedefflags", 0);
			
			// Get map lumps
			maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

			// Get texture and flat sources
			textureranges = cfg.ReadSetting("textures", new Hashtable());
			flatranges = cfg.ReadSetting("flats", new Hashtable());
			
			// Get thing categories
			dic = cfg.ReadSetting("thingtypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make a category
				thingcat = new ThingCategory(cfg, de.Key.ToString());

				// Add all thing in category to the big list
				foreach(ThingTypeInfo t in thingcat.Things) things.Add(t.Index, t);
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// ReadSetting
		public string ReadSetting(string setting, string defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public int ReadSetting(string setting, int defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public float ReadSetting(string setting, float defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public short ReadSetting(string setting, short defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public long ReadSetting(string setting, long defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public bool ReadSetting(string setting, bool defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public byte ReadSetting(string setting, byte defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public IDictionary ReadSetting(string setting, IDictionary defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		
		// This gets thing information by index
		public ThingTypeInfo GetThingInfo(int thingtype)
		{
			// Index in config?
			if(things.ContainsKey(thingtype))
			{
				// Return from config
				return things[thingtype];
			}
			else
			{
				// Create unknown thing info
				return new ThingTypeInfo(thingtype);
			}
		}
		
		#endregion
	}
}
