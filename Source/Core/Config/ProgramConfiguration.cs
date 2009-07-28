
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
using CodeImp.DoomBuilder.Map;
using System.Reflection;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ProgramConfiguration
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Original configuration
		private Configuration cfg;
		
		// Cached variables
		//private int undolevels;
		private bool blackbrowsers;
		private int visualfov;
		private float visualmousesensx;
		private float visualmousesensy;
		private int imagebrightness;
		private float doublesidedalpha;
		private byte doublesidedalphabyte;
		private float backgroundalpha;
		private bool qualitydisplay;
		private bool squarethings;
		private bool testmonsters;
		private int defaultviewmode;
		private bool classicbilinear;
		private bool visualbilinear;
		private int mousespeed;
		private int movespeed;
		private float viewdistance;
		private bool invertyaxis;
		private string scriptfontname;
		private int scriptfontsize;
		private bool scriptfontbold;
		private bool scriptontop;
		private bool scriptautoindent;
		private int scripttabwidth;
		private int previewimagesize;
		private int autoscrollspeed;
		private bool showerrorswindow;
		private bool animatevisualselection;
		private int previousversion;
		private PasteOptions pasteoptions;
		private int dockersposition;
		private bool collapsedockers;
		private int dockerswidth;
		
		// These are not stored in the configuration, only used at runtime
		private string defaulttexture;
		private int defaultbrightness = 192;
		private int defaultfloorheight = 0;
		private int defaultceilheight = 128;
		private string defaultfloortexture;
		private string defaultceiltexture;
		private int defaultthingtype = 1;
		private float defaultthingangle = 0.0f;
		private List<string> defaultthingflags;
		
		#endregion

		#region ================== Properties

		internal Configuration Config { get { return cfg; } }
		//public int UndoLevels { get { return undolevels; } internal set { undolevels = value; } }
		public bool BlackBrowsers { get { return blackbrowsers; } internal set { blackbrowsers = value; } }
		public int VisualFOV { get { return visualfov; } internal set { visualfov = value; } }
		public int ImageBrightness { get { return imagebrightness; } internal set { imagebrightness = value; } }
		public float DoubleSidedAlpha { get { return doublesidedalpha; } internal set { doublesidedalpha = value; doublesidedalphabyte = (byte)(doublesidedalpha * 255f); } }
		public byte DoubleSidedAlphaByte { get { return doublesidedalphabyte; } }
		public float BackgroundAlpha { get { return backgroundalpha; } internal set { backgroundalpha = value; } }
		public float VisualMouseSensX { get { return visualmousesensx; } internal set { visualmousesensx = value; } }
		public float VisualMouseSensY { get { return visualmousesensy; } internal set { visualmousesensy = value; } }
		public bool QualityDisplay { get { return qualitydisplay; } internal set { qualitydisplay = value; } }
		public bool SquareThings { get { return squarethings; } internal set { squarethings = value; } }
		public bool TestMonsters { get { return testmonsters; } internal set { testmonsters = value; } }
		public int DefaultViewMode { get { return defaultviewmode; } internal set { defaultviewmode = value; } }
		public bool ClassicBilinear { get { return classicbilinear; } internal set { classicbilinear = value; } }
		public bool VisualBilinear { get { return visualbilinear; } internal set { visualbilinear = value; } }
		public int MouseSpeed { get { return mousespeed; } internal set { mousespeed = value; } }
		public int MoveSpeed { get { return movespeed; } internal set { movespeed = value; } }
		public float ViewDistance { get { return viewdistance; } internal set { viewdistance = value; } }
		public bool InvertYAxis { get { return invertyaxis; } internal set { invertyaxis = value; } }
		public string ScriptFontName { get { return scriptfontname; } internal set { scriptfontname = value; } }
		public int ScriptFontSize { get { return scriptfontsize; } internal set { scriptfontsize = value; } }
		public bool ScriptFontBold { get { return scriptfontbold; } internal set { scriptfontbold = value; } }
		public bool ScriptOnTop { get { return scriptontop; } internal set { scriptontop = value; } }
		public int PreviewImageSize { get { return previewimagesize; } internal set { previewimagesize = value; } }
		public int AutoScrollSpeed { get { return autoscrollspeed; } internal set { autoscrollspeed = value; } }
		public bool ShowErrorsWindow { get { return showerrorswindow; } internal set { showerrorswindow = value; } }
		public bool AnimateVisualSelection { get { return animatevisualselection; } internal set { animatevisualselection = value; } }
		public int ScriptTabWidth { get { return scripttabwidth; } internal set { scripttabwidth = value; } }
		public bool ScriptAutoIndent { get { return scriptautoindent; } internal set { scriptautoindent = value; } }
		internal int PreviousVersion { get { return previousversion; } }
		internal PasteOptions PasteOptions { get { return pasteoptions; } set { pasteoptions = value; } }
		public int DockersPosition { get { return dockersposition; } internal set { dockersposition = value; } }
		public bool CollapseDockers { get { return collapsedockers; } internal set { collapsedockers = value; } }
		public int DockersWidth { get { return dockerswidth; } internal set { dockerswidth = value; } }
		
		public string DefaultTexture { get { return defaulttexture; } set { defaulttexture = value; } }
		public string DefaultFloorTexture { get { return defaultfloortexture; } set { defaultfloortexture = value; } }
		public string DefaultCeilingTexture { get { return defaultceiltexture; } set { defaultceiltexture = value; } }
		public int DefaultBrightness { get { return defaultbrightness; } set { defaultbrightness = value; } }
		public int DefaultFloorHeight { get { return defaultfloorheight; } set { defaultfloorheight = value; } }
		public int DefaultCeilingHeight { get { return defaultceilheight; } set { defaultceilheight = value; } }
		public int DefaultThingType { get { return defaultthingtype; } set { defaultthingtype = value; } }
		public float DefaultThingAngle { get { return defaultthingangle; } set { defaultthingangle = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ProgramConfiguration()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
			defaultthingflags = new List<string>();
			pasteoptions = new PasteOptions();
		}

		#endregion

		#region ================== Loading / Saving

		// This loads the program configuration
		internal bool Load(string cfgfilepathname, string defaultfilepathname)
		{
			// First parse it
			if(Read(cfgfilepathname, defaultfilepathname))
			{
				// Read the cache variables
				blackbrowsers = cfg.ReadSetting("blackbrowsers", false);
				//undolevels = cfg.ReadSetting("undolevels", 20);
				visualfov = cfg.ReadSetting("visualfov", 80);
				visualmousesensx = cfg.ReadSetting("visualmousesensx", 40f);
				visualmousesensy = cfg.ReadSetting("visualmousesensy", 40f);
				imagebrightness = cfg.ReadSetting("imagebrightness", 3);
				doublesidedalpha = cfg.ReadSetting("doublesidedalpha", 0.4f);
				doublesidedalphabyte = (byte)(doublesidedalpha * 255f);
				backgroundalpha = cfg.ReadSetting("backgroundalpha", 1.0f);
				qualitydisplay = cfg.ReadSetting("qualitydisplay", true);
				squarethings = cfg.ReadSetting("squarethings", false);
				testmonsters = cfg.ReadSetting("testmonsters", true);
				defaultviewmode = cfg.ReadSetting("defaultviewmode", (int)ViewMode.Normal);
				classicbilinear = cfg.ReadSetting("classicbilinear", false);
				visualbilinear = cfg.ReadSetting("visualbilinear", false);
				mousespeed = cfg.ReadSetting("mousespeed", 100);
				movespeed = cfg.ReadSetting("movespeed", 100);
				viewdistance = cfg.ReadSetting("viewdistance", 3000.0f);
				invertyaxis = cfg.ReadSetting("invertyaxis", false);
				scriptfontname = cfg.ReadSetting("scriptfontname", "Lucida Console");
				scriptfontsize = cfg.ReadSetting("scriptfontsize", 10);
				scriptfontbold = cfg.ReadSetting("scriptfontbold", false);
				scriptautoindent = cfg.ReadSetting("scriptautoindent", true);
				scriptontop = cfg.ReadSetting("scriptontop", true);
				scripttabwidth = cfg.ReadSetting("scripttabwidth", 4);
				previewimagesize = cfg.ReadSetting("previewimagesize", 1);
				autoscrollspeed = cfg.ReadSetting("autoscrollspeed", 0);
				showerrorswindow = cfg.ReadSetting("showerrorswindow", true);
				animatevisualselection = cfg.ReadSetting("animatevisualselection", true);
				previousversion = cfg.ReadSetting("currentversion", 0);
				dockersposition = cfg.ReadSetting("dockersposition", 1);
				collapsedockers = cfg.ReadSetting("collapsedockers", true);
				dockerswidth = cfg.ReadSetting("dockerswidth", 300);
				pasteoptions.ReadConfiguration(cfg, "pasteoptions");
				
				// Success
				return true;
			}
			else
			{
				// Failed
				return false;
			}
		}

		// This saves the program configuration
		internal void Save(string filepathname)
		{
			Version v = General.ThisAssembly.GetName().Version;
			
			// Write the cache variables
			cfg.WriteSetting("blackbrowsers", blackbrowsers);
			//cfg.WriteSetting("undolevels", undolevels);
			cfg.WriteSetting("visualfov", visualfov);
			cfg.WriteSetting("visualmousesensx", visualmousesensx);
			cfg.WriteSetting("visualmousesensy", visualmousesensy);
			cfg.WriteSetting("imagebrightness", imagebrightness);
			cfg.WriteSetting("qualitydisplay", qualitydisplay);
			cfg.WriteSetting("squarethings", squarethings);
			cfg.WriteSetting("testmonsters", testmonsters);
			cfg.WriteSetting("doublesidedalpha", doublesidedalpha);
			cfg.WriteSetting("backgroundalpha", backgroundalpha);
			cfg.WriteSetting("defaultviewmode", defaultviewmode);
			cfg.WriteSetting("classicbilinear", classicbilinear);
			cfg.WriteSetting("visualbilinear", visualbilinear);
			cfg.WriteSetting("mousespeed", mousespeed);
			cfg.WriteSetting("movespeed", movespeed);
			cfg.WriteSetting("viewdistance", viewdistance);
			cfg.WriteSetting("invertyaxis", invertyaxis);
			cfg.WriteSetting("scriptfontname", scriptfontname);
			cfg.WriteSetting("scriptfontsize", scriptfontsize);
			cfg.WriteSetting("scriptfontbold", scriptfontbold);
			cfg.WriteSetting("scriptontop", scriptontop);
			cfg.WriteSetting("scripttabwidth", scripttabwidth);
			cfg.WriteSetting("scriptautoindent", scriptautoindent);
			cfg.WriteSetting("previewimagesize", previewimagesize);
			cfg.WriteSetting("autoscrollspeed", autoscrollspeed);
			cfg.WriteSetting("showerrorswindow", showerrorswindow);
			cfg.WriteSetting("animatevisualselection", animatevisualselection);
			cfg.WriteSetting("currentversion", v.Major * 1000000 + v.Revision);
			cfg.WriteSetting("dockersposition", dockersposition);
			cfg.WriteSetting("collapsedockers", collapsedockers);
			cfg.WriteSetting("dockerswidth", dockerswidth);
			pasteoptions.WriteConfiguration(cfg, "pasteoptions");
			
			// Save settings configuration
			General.WriteLogLine("Saving program configuration...");
			cfg.SaveConfiguration(filepathname);
		}
		
		// This reads the configuration
		private bool Read(string cfgfilepathname, string defaultfilepathname)
		{
			DialogResult result;

			// Check if no config for this user exists yet
			if(!File.Exists(cfgfilepathname))
			{
				// Copy new configuration
				General.WriteLogLine("Local user program configuration is missing!");
				File.Copy(defaultfilepathname, cfgfilepathname);
				General.WriteLogLine("New program configuration copied for local user");
			}

			// Load it
			cfg = new Configuration(cfgfilepathname, true);
			if(cfg.ErrorResult)
			{
				// Error in configuration
				// Ask user for a new copy
				result = General.ShowErrorMessage("Error in program configuration near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription + "\n\nWould you like to overwrite your settings with a new configuration to restore the default settings?", MessageBoxButtons.YesNoCancel);
				if(result == DialogResult.Yes)
				{
					// Remove old configuration and make a new copy
					General.WriteLogLine("User requested a new copy of the program configuration");
					File.Delete(cfgfilepathname);
					File.Copy(defaultfilepathname, cfgfilepathname);
					General.WriteLogLine("New program configuration copied for local user");

					// Load it
					cfg = new Configuration(cfgfilepathname, true);
					if(cfg.ErrorResult)
					{
						// Error in configuration
						General.WriteLogLine("Error in program configuration near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
						General.ShowErrorMessage("Default program configuration is corrupted. Please re-install Doom Builder.", MessageBoxButtons.OK);
						return false;
					}
				}
				else if(result == DialogResult.Cancel)
				{
					// User requested to cancel startup
					General.WriteLogLine("User cancelled startup");
					return false;
				}
			}
			
			// Check if a version number is missing
			previousversion = cfg.ReadSetting("currentversion", -1);
			if(previousversion == -1)
			{
				// Remove old configuration and make a new copy
				General.WriteLogLine("Program configuration is outdated, new configuration will be copied for local user");
				File.Delete(cfgfilepathname);
				File.Copy(defaultfilepathname, cfgfilepathname);
				
				// Load it
				cfg = new Configuration(cfgfilepathname, true);
				if(cfg.ErrorResult)
				{
					// Error in configuration
					General.WriteLogLine("Error in program configuration near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					General.ShowErrorMessage("Default program configuration is corrupted. Please re-install Doom Builder.", MessageBoxButtons.OK);
					return false;
				}
			}
			
			// Success
			return true;
		}
		
		#endregion

		#region ================== Methods

		// This makes the path prefix for the given assembly
		private string GetPluginPathPrefix(Assembly asm)
		{
			Plugin p = General.Plugins.FindPluginByAssembly(asm);
			return "plugins." + p.Name.ToLowerInvariant() + ".";
		}

		// ReadPluginSetting
		public string ReadPluginSetting(string setting, string defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public int ReadPluginSetting(string setting, int defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public float ReadPluginSetting(string setting, float defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public short ReadPluginSetting(string setting, short defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public long ReadPluginSetting(string setting, long defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public bool ReadPluginSetting(string setting, bool defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public byte ReadPluginSetting(string setting, byte defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }
		public IDictionary ReadPluginSetting(string setting, IDictionary defaultsetting) { return cfg.ReadSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, defaultsetting); }

		// ReadPluginSetting with specific plugin
		public string ReadPluginSetting(string pluginname, string setting, string defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public int ReadPluginSetting(string pluginname, string setting, int defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public float ReadPluginSetting(string pluginname, string setting, float defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public short ReadPluginSetting(string pluginname, string setting, short defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public long ReadPluginSetting(string pluginname, string setting, long defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public bool ReadPluginSetting(string pluginname, string setting, bool defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public byte ReadPluginSetting(string pluginname, string setting, byte defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		public IDictionary ReadPluginSetting(string pluginname, string setting, IDictionary defaultsetting) { return cfg.ReadSetting(pluginname.ToLowerInvariant() + "." + setting, defaultsetting); }
		
		// WritePluginSetting
		public bool WritePluginSetting(string setting, object settingvalue) { return cfg.WriteSetting(GetPluginPathPrefix(Assembly.GetCallingAssembly()) + setting, settingvalue); }
		
		// ReadSetting
		internal string ReadSetting(string setting, string defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal int ReadSetting(string setting, int defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal float ReadSetting(string setting, float defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal short ReadSetting(string setting, short defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal long ReadSetting(string setting, long defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal bool ReadSetting(string setting, bool defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal byte ReadSetting(string setting, byte defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		internal IDictionary ReadSetting(string setting, IDictionary defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }

		// WriteSetting
		internal bool WriteSetting(string setting, object settingvalue) { return cfg.WriteSetting(setting, settingvalue); }
		internal bool WriteSetting(string setting, object settingvalue, string pathseperator) { return cfg.WriteSetting(setting, settingvalue, pathseperator); }

		#endregion

		#region ================== Default Settings

		// This sets the default thing flags
		public void SetDefaultThingFlags(ICollection<string> setflags)
		{
			defaultthingflags = new List<string>(setflags);
		}

		// This applies default settings to a thing
		public void ApplyDefaultThingSettings(Thing t)
		{
			t.Type = defaultthingtype;
			t.Rotate(defaultthingangle);
			foreach(string f in defaultthingflags) t.SetFlag(f, true);
		}
		
		// This attempts to find the default drawing settings
		public void FindDefaultDrawSettings()
		{
			bool foundone;
			
			// Only possible when a map is loaded
			if(General.Map == null) return;
			
			// Default texture missing?
			if((defaulttexture == null) || defaulttexture.StartsWith("-"))
			{
				// Find default texture from map
				foundone = false;
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
				{
					if(!sd.MiddleTexture.StartsWith("-"))
					{
						foundone = true;
						defaulttexture = sd.MiddleTexture;
						break;
					}
				}
				
				// Not found yet?
				if(!foundone)
				{
					// Pick the first STARTAN from the list.
					// I love the STARTAN texture as default for some reason.
					foreach(string s in General.Map.Data.TextureNames)
					{
						if(s.StartsWith("STARTAN"))
						{
							foundone = true;
							defaulttexture = s;
							break;
						}
					}
					
					// Otherwise just pick the first
					if(!foundone)
					{
						if(General.Map.Data.TextureNames.Count > 1)
							defaulttexture = General.Map.Data.TextureNames[1];
					}
				}
			}

			// Default floor missing?
			if((defaultfloortexture == null) || (defaultfloortexture.Length == 0))
			{
				// Find default texture from map
				foundone = false;
				if(General.Map.Map.Sectors.Count > 0)
				{
					foundone = true;
					defaultfloortexture = General.GetByIndex(General.Map.Map.Sectors, 0).FloorTexture;
				}

				// Pick the first FLOOR from the list.
				if(!foundone)
				{
					foreach(string s in General.Map.Data.FlatNames)
					{
						if(s.StartsWith("FLOOR"))
						{
							foundone = true;
							defaultfloortexture = s;
							break;
						}
					}
				}
				
				// Otherwise just pick the first
				if(!foundone)
				{
					if(General.Map.Data.FlatNames.Count > 0)
						defaultfloortexture = General.Map.Data.FlatNames[0];
				}
			}

			// Default ceiling missing?
			if((defaultceiltexture == null) || (defaultceiltexture.Length == 0))
			{
				// Find default texture from map
				foundone = false;
				if(General.Map.Map.Sectors.Count > 0)
				{
					foundone = true;
					defaultceiltexture = General.GetByIndex(General.Map.Map.Sectors, 0).CeilTexture;
				}

				// Pick the first CEIL from the list.
				if(!foundone)
				{
					foreach(string s in General.Map.Data.FlatNames)
					{
						if(s.StartsWith("CEIL"))
						{
							foundone = true;
							defaultceiltexture = s;
							break;
						}
					}
				}
				
				// Otherwise just pick the first
				if(!foundone)
				{
					if(General.Map.Data.FlatNames.Count > 1)
						defaultceiltexture = General.Map.Data.FlatNames[1];
				}
			}

			// Texture names may not be null
			if((defaulttexture == null) || (defaulttexture == "")) defaulttexture = "-";
			if((defaultfloortexture == null) || (defaultfloortexture == "")) defaultfloortexture = "-";
			if((defaultceiltexture == null) || (defaultceiltexture == "")) defaultceiltexture = "-";
		}
		
		#endregion
	}
}
