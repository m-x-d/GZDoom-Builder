
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
		private string configname;
		private string enginename;
		private float defaulttexturescale;
		private float defaultflatscale;
		private string formatinterface;
		private string soundlinedefflag;
		private string singlesidedflag;
		private string doublesidedflag;
		private string impassableflag;
		private bool mixtexturesflats;
		private bool generalizedactions;
		private bool generalizedeffects;
		private int start3dmodethingtype;
		private int linedefactivationsfilter;
		private string testparameters;
		
		// Skills
		private List<SkillInfo> skills;

		// Map lumps
		private IDictionary maplumpnames;
		
		// Texture/flat sources
		private IDictionary textureranges;
		private IDictionary flatranges;
		
		// Things
		private List<string> defaultthingflags;
		private Dictionary<string, string> thingflags;
		private List<ThingCategory> thingcategories;
		private Dictionary<int, ThingTypeInfo> things;
		
		// Linedefs
		private Dictionary<string, string> linedefflags;
		private Dictionary<int, LinedefActionInfo> linedefactions;
		private List<LinedefActionInfo> sortedlinedefactions;
		private List<LinedefActionCategory> actioncategories;
		private List<LinedefActivateInfo> linedefactivates;
		private List<GeneralizedCategory> genactioncategories;
		
		// Sectors
		private Dictionary<int, SectorEffectInfo> sectoreffects;
		private List<SectorEffectInfo> sortedsectoreffects;
		private List<GeneralizedOption> geneffectoptions;

		// Universal fields
		private List<UniversalFieldInfo> linedeffields;
		private List<UniversalFieldInfo> sectorfields;
		private List<UniversalFieldInfo> sidedeffields;
		private List<UniversalFieldInfo> thingfields;
		private List<UniversalFieldInfo> vertexfields;
		
		// Enums
		private Dictionary<string, EnumList> enums;
		
		// Default Texture Sets
		private List<DefinedTextureSet> texturesets;
		
		#endregion

		#region ================== Properties

		// General settings
		public string Name { get { return configname; } }
		public string EngineName { get { return enginename; } }
		public float DefaultTextureScale { get { return defaulttexturescale; } }
		public float DefaultFlatScale { get { return defaultflatscale; } }
		public string FormatInterface { get { return formatinterface; } }
		public string SoundLinedefFlag { get { return soundlinedefflag; } }
		public string SingleSidedFlag { get { return singlesidedflag; } }
		public string DoubleSidedFlag { get { return doublesidedflag; } }
		public string ImpassableFlag { get { return impassableflag; } }
		public bool MixTexturesFlats { get { return mixtexturesflats; } }
		public bool GeneralizedActions { get { return generalizedactions; } }
		public bool GeneralizedEffects { get { return generalizedeffects; } }
		public int Start3DModeThingType { get { return start3dmodethingtype; } }
		public int LinedefActivationsFilter { get { return linedefactivationsfilter; } }
		public string TestParameters { get { return testparameters; } }
		
		// Skills
		public List<SkillInfo> Skills { get { return skills; } }
		
		// Map lumps
		public IDictionary MapLumpNames { get { return maplumpnames; } }

		// Texture/flat sources
		public IDictionary TextureRanges { get { return textureranges; } }
		public IDictionary FlatRanges { get { return flatranges; } }

		// Things
		public ICollection<string> DefaultThingFlags { get { return defaultthingflags; } }
		public IDictionary<string, string> ThingFlags { get { return thingflags; } }
		public List<ThingCategory> ThingCategories { get { return thingcategories; } }
		public ICollection<ThingTypeInfo> Things { get { return things.Values; } }
		
		// Linedefs
		public IDictionary<string, string> LinedefFlags { get { return linedefflags; } }
		public IDictionary<int, LinedefActionInfo> LinedefActions { get { return linedefactions; } }
		public List<LinedefActionInfo> SortedLinedefActions { get { return sortedlinedefactions; } }
		public List<LinedefActionCategory> ActionCategories { get { return actioncategories; } }
		public List<LinedefActivateInfo> LinedefActivates { get { return linedefactivates; } }
		public List<GeneralizedCategory> GenActionCategories { get { return genactioncategories; } }

		// Sectors
		public IDictionary<int, SectorEffectInfo> SectorEffects { get { return sectoreffects; } }
		public List<SectorEffectInfo> SortedSectorEffects { get { return sortedsectoreffects; } }
		public List<GeneralizedOption> GenEffectOptions { get { return geneffectoptions; } }

		// Universal fields
		public List<UniversalFieldInfo> LinedefFields { get { return linedeffields; } }
		public List<UniversalFieldInfo> SectorFields { get { return sectorfields; } }
		public List<UniversalFieldInfo> SidedefFields { get { return sidedeffields; } }
		public List<UniversalFieldInfo> ThingFields { get { return thingfields; } }
		public List<UniversalFieldInfo> VertexFields { get { return vertexfields; } }

		// Enums
		public IDictionary<string, EnumList> Enums { get { return enums; } }

		// Texture Sets
		internal List<DefinedTextureSet> TextureSets { get { return texturesets; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal GameConfiguration(Configuration cfg)
		{
			object obj;
			
			// Initialize
			this.cfg = cfg;
			this.thingflags = new Dictionary<string, string>();
			this.defaultthingflags = new List<string>();
			this.thingcategories = new List<ThingCategory>();
			this.things = new Dictionary<int, ThingTypeInfo>();
			this.linedefflags = new Dictionary<string, string>();
			this.linedefactions = new Dictionary<int, LinedefActionInfo>();
			this.actioncategories = new List<LinedefActionCategory>();
			this.sortedlinedefactions = new List<LinedefActionInfo>();
			this.linedefactivates = new List<LinedefActivateInfo>();
			this.genactioncategories = new List<GeneralizedCategory>();
			this.sectoreffects = new Dictionary<int, SectorEffectInfo>();
			this.sortedsectoreffects = new List<SectorEffectInfo>();
			this.geneffectoptions = new List<GeneralizedOption>();
			this.enums = new Dictionary<string, EnumList>();
			this.skills = new List<SkillInfo>();
			this.texturesets = new List<DefinedTextureSet>();
			
			// Read general settings
			configname = cfg.ReadSetting("game", "<unnamed game>");
			enginename = cfg.ReadSetting("engine", "");
			defaulttexturescale = cfg.ReadSetting("defaulttexturescale", 1f);
			defaultflatscale = cfg.ReadSetting("defaultflatscale", 1f);
			formatinterface = cfg.ReadSetting("formatinterface", "");
			mixtexturesflats = cfg.ReadSetting("mixtexturesflats", false);
			generalizedactions = cfg.ReadSetting("generalizedlinedefs", false);
			generalizedeffects = cfg.ReadSetting("generalizedsectors", false);
			start3dmodethingtype = cfg.ReadSetting("start3dmode", 0);
			linedefactivationsfilter = cfg.ReadSetting("linedefactivationsfilter", 0);
			testparameters = cfg.ReadSetting("testparameters", "");
			
			// Flags have special (invariant culture) conversion
			// because they are allowed to be written as integers in the configs
			obj = cfg.ReadSettingObject("soundlinedefflag", 0);
			if(obj is int) soundlinedefflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else soundlinedefflag = obj.ToString();
			obj = cfg.ReadSettingObject("singlesidedflag", 0);
			if(obj is int) singlesidedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else singlesidedflag = obj.ToString();
			obj = cfg.ReadSettingObject("doublesidedflag", 0);
			if(obj is int) doublesidedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else doublesidedflag = obj.ToString();
			obj = cfg.ReadSettingObject("impassableflag", 0);
			if(obj is int) impassableflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else impassableflag = obj.ToString();
			
			// Get map lumps
			maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

			// Get texture and flat sources
			textureranges = cfg.ReadSetting("textures", new Hashtable());
			flatranges = cfg.ReadSetting("flats", new Hashtable());
			
			// Skills
			LoadSkills();

			// Enums
			LoadEnums();
			
			// Things
			LoadThingFlags();
			LoadDefaultThingFlags();
			LoadThingCategories();
			
			// Linedefs
			LoadLinedefFlags();
			LoadLinedefActions();
			LoadLinedefActivations();
			LoadLinedefGeneralizedActions();

			// Sectors
			LoadSectorEffects();
			LoadSectorGeneralizedEffects();
			
			// Universal fields
			linedeffields = LoadUniversalFields("linedef");
			sectorfields = LoadUniversalFields("sector");
			sidedeffields = LoadUniversalFields("sidedef");
			thingfields = LoadUniversalFields("thing");
			vertexfields = LoadUniversalFields("vertex");

			// Texture sets
			LoadTextureSets();
		}

		// Destructor
		~GameConfiguration()
		{
			foreach(ThingCategory tc in thingcategories) tc.Dispose();
			foreach(LinedefActionCategory ac in actioncategories) ac.Dispose();
		}
		
		#endregion

		#region ================== Loading

		// This loads the enumerations
		private void LoadEnums()
		{
			IDictionary dic;

			// Get enums list
			dic = cfg.ReadSetting("enums", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make new enum
				EnumList list = new EnumList(de.Key.ToString(), cfg);
				enums.Add(de.Key.ToString(), list);
			}
		}
		
		// This loads a universal fields list
		private List<UniversalFieldInfo> LoadUniversalFields(string elementname)
		{
			List<UniversalFieldInfo> list = new List<UniversalFieldInfo>();
			UniversalFieldInfo uf;
			IDictionary dic;
			
			// Get fields
			dic = cfg.ReadSetting("universalfields." + elementname, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				try
				{
					// Read the field info and add to list
					uf = new UniversalFieldInfo(elementname, de.Key.ToString(), cfg, enums);
					list.Add(uf);
				}
				catch(Exception)
				{
					General.WriteLogLine("WARNING: Unable to read universal field definition 'universalfields." + elementname + "." + de.Key + "'!");
				}
			}

			// Return result
			return list;
		}
		
		// Things and thing categories
		private void LoadThingCategories()
		{
			IDictionary dic;
			ThingCategory thingcat;
			
			// Get thing categories
			dic = cfg.ReadSetting("thingtypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make a category
				thingcat = new ThingCategory(cfg, de.Key.ToString());

				// Add all things in category to the big list
				foreach(ThingTypeInfo t in thingcat.Things)
				{
					if(!things.ContainsKey(t.Index))
					{
						things.Add(t.Index, t);
					}
					else
					{
						General.WriteLogLine("WARNING: Thing number " + t.Index + " is defined more than once! (as '" + things[t.Index].Title + "' and '" + t.Title + "')");
					}
				}

				// Add category to list
				thingcategories.Add(thingcat);
			}
		}
		
		// Linedef flags
		private void LoadLinedefFlags()
		{
			IDictionary dic;
			int bitflagscheck = 0;
			int bitvalue;
			
			// Get linedef flags
			dic = cfg.ReadSetting("linedefflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				/*
				// Try paring the bit value
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out bitvalue))
				{
					// Check for conflict and add to list
					if((bitvalue & bitflagscheck) == 0)
						linedefflags.Add(bitvalue, de.Value.ToString());
					else
						General.WriteLogLine("WARNING: Structure 'linedefflags' contains conflicting bit flag keys. Make sure all keys are unique integers and powers of 2!");
						
					// Update bit flags checking value
					bitflagscheck |= bitvalue;
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'linedefflags' contains invalid keys!");
				}
				*/
				
				linedefflags.Add(de.Key.ToString(), de.Value.ToString());
			}
		}

		// Linedef actions and action categories
		private void LoadLinedefActions()
		{
			Dictionary<string, LinedefActionCategory> cats = new Dictionary<string, LinedefActionCategory>();
			IDictionary dic;
			LinedefActionInfo ai;
			LinedefActionCategory ac;
			int actionnumber;
			
			// Get linedef categories
			dic = cfg.ReadSetting("linedeftypes", new Hashtable());
			foreach(DictionaryEntry cde in dic)
			{
				// Read category title
				string cattitle = cfg.ReadSetting("linedeftypes." + cde.Key + ".title", "");

				// Make or get category
				if(cats.ContainsKey(cde.Key.ToString()))
					ac = cats[cde.Key.ToString()];
				else
				{
					ac = new LinedefActionCategory(cde.Key.ToString(), cattitle);
					cats.Add(cde.Key.ToString(), ac);
				}
				
				// Go for all line types in category
				IDictionary catdic = cfg.ReadSetting("linedeftypes." + cde.Key, new Hashtable());
				foreach(DictionaryEntry de in catdic)
				{
					// Check if the item key is numeric
					if(int.TryParse(de.Key.ToString(),
						NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
						CultureInfo.InvariantCulture, out actionnumber))
					{
						// Check if the item value is a structure
						if(de.Value is IDictionary)
						{
							// Make the line type
							ai = new LinedefActionInfo(actionnumber, cfg, cde.Key.ToString(), enums);

							// Add action to category and sorted list
							sortedlinedefactions.Add(ai);
							linedefactions.Add(actionnumber, ai);
							ac.Add(ai);
						}
						else
						{
							// Failure
							General.WriteLogLine("WARNING: Structure 'linedeftypes' contains invalid types! (all types must be expanded structures)");
						}
					}
				}
			}

			// Sort the actions list
			sortedlinedefactions.Sort();
			
			// Copy categories to final list
			actioncategories.Clear();
			actioncategories.AddRange(cats.Values);

			// Sort the categories list
			actioncategories.Sort();
		}

		// Linedef activates
		private void LoadLinedefActivations()
		{
			IDictionary dic;
			int bitvalue;

			// Get linedef activations
			dic = cfg.ReadSetting("linedefactivations", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Add to the list
				linedefactivates.Add(new LinedefActivateInfo(de.Key.ToString(), de.Value.ToString()));
			}

			// Sort the list
			linedefactivates.Sort();
		}

		// Linedef generalized actions
		private void LoadLinedefGeneralizedActions()
		{
			IDictionary dic;

			// Get linedef activations
			dic = cfg.ReadSetting("gen_linedeftypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				if(de.Value is IDictionary)
				{
					// Add category
					genactioncategories.Add(new GeneralizedCategory("gen_linedeftypes", de.Key.ToString(), cfg));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'gen_linedeftypes' contains invalid entries!");
				}
			}
		}

		// Sector effects
		private void LoadSectorEffects()
		{
			IDictionary dic;
			SectorEffectInfo si;
			int actionnumber;
			
			// Get sector effects
			dic = cfg.ReadSetting("sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the action number
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out actionnumber))
				{
					// Make effects
					si = new SectorEffectInfo(actionnumber, de.Value.ToString());
					
					// Add action to category and sorted list
					sortedsectoreffects.Add(si);
					sectoreffects.Add(actionnumber, si);
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'sectortypes' contains invalid keys!");
				}
			}

			// Sort the actions list
			sortedsectoreffects.Sort();
		}

		// Sector generalized effects
		private void LoadSectorGeneralizedEffects()
		{
			IDictionary dic;

			// Get sector effects
			dic = cfg.ReadSetting("gen_sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				if(de.Value is IDictionary)
				{
					// Add option
					geneffectoptions.Add(new GeneralizedOption("gen_sectortypes", "", de.Key.ToString(), de.Value as IDictionary));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'gen_sectortypes' contains invalid entries!");
				}
			}
		}

		// Thing flags
		private void LoadThingFlags()
		{
			IDictionary dic;

			// Get linedef flags
			dic = cfg.ReadSetting("thingflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
				thingflags.Add(de.Key.ToString(), de.Value.ToString());
		}

		// Default thing flags
		private void LoadDefaultThingFlags()
		{
			IDictionary dic;

			// Get linedef flags
			dic = cfg.ReadSetting("defaultthingflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if flag exists
				if(thingflags.ContainsKey(de.Key.ToString()))
				{
					defaultthingflags.Add(de.Key.ToString());
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'defaultthingflags' contains unknown thing flags!");
				}
			}
		}

		// Skills
		private void LoadSkills()
		{
			IDictionary dic;

			// Get skills
			dic = cfg.ReadSetting("skills", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				int num = 0;
				if(int.TryParse(de.Key.ToString(), out num))
				{
					skills.Add(new SkillInfo(num, de.Value.ToString()));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'skills' contains invalid skill numbers!");
				}
			}
		}
		
		// Texture Sets
		private void LoadTextureSets()
		{
			IDictionary dic;

			// Get sets
			dic = cfg.ReadSetting("texturesets", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				DefinedTextureSet s = new DefinedTextureSet(cfg, "texturesets." + de.Key.ToString());
				texturesets.Add(s);
			}
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

		// This gets thing information by index
		// Returns null when thing type info could not be found
		public ThingTypeInfo GetThingInfoEx(int thingtype)
		{
			// Index in config?
			if(things.ContainsKey(thingtype))
			{
				// Return from config
				return things[thingtype];
			}
			else
			{
				// No such thing type known
				return null;
			}
		}
		
		// This checks if an action is generalized or predefined
		public static bool IsGeneralized(int action, List<GeneralizedCategory> categories)
		{
			// Only actions above 0
			if(action > 0)
			{
				// Go for all categories
				foreach(GeneralizedCategory ac in categories)
				{
					// Check if the action is within range of this category
					if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) return true;
				}
			}

			// Not generalized
			return false;
		}

		// This gets the generalized action category from action number
		public GeneralizedCategory GetGeneralizedActionCategory(int action)
		{
			// Only actions above 0
			if(action > 0)
			{
				// Go for all categories
				foreach(GeneralizedCategory ac in genactioncategories)
				{
					// Check if the action is within range of this category
					if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) return ac;
				}
			}

			// Not generalized
			return null;
		}
		
		// This checks if a specific edit mode class is listed
		public bool IsEditModeSpecified(string classname)
		{
			return cfg.SettingExists("editingmodes." + classname.ToString(CultureInfo.InvariantCulture));
		}
		
		#endregion
	}
}
