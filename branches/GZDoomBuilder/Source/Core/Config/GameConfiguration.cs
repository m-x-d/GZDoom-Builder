
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class GameConfiguration
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Original configuration
		private readonly Configuration cfg;
		
		// General settings
		private readonly string configname;
		private readonly string enginename;
		private readonly float defaulttexturescale;
		private readonly float defaultflatscale;
		private readonly string defaultwalltexture; //mxd
		private readonly string defaultfloortexture; //mxd
		private readonly string defaultceilingtexture; //mxd
		private readonly bool scaledtextureoffsets;
		private readonly string defaultsavecompiler;
		private readonly string defaulttestcompiler;
		private readonly string formatinterface;
		private readonly string defaultlinedefactivation; //mxd
		private readonly string singlesidedflag;
		private readonly string doublesidedflag;
		private readonly string impassableflag;
		private readonly string upperunpeggedflag;
		private readonly string lowerunpeggedflag;
		private readonly bool mixtexturesflats;
		private readonly bool generalizedactions;
		private readonly bool generalizedeffects;
		private readonly int start3dmodethingtype;
		private readonly int linedefactivationsfilter;
		private readonly string testparameters;
		private readonly bool testshortpaths;
		private readonly string makedoortrack;
		private readonly string makedoordoor; //mxd
		private readonly string makedoorceil; //mxd
		private readonly int makedooraction;
		private readonly int makedooractivate;
		private readonly int[] makedoorargs;
		private readonly Dictionary<string, bool> makedoorflags;
		private readonly bool linetagindicatesectors;
		private readonly string decorategames;
		private string skyflatname;
		private readonly Dictionary<string, string> defaultskytextures; //mxd <map name, sky texture name>
		private readonly int maxtexturenamelength;
		private readonly bool longtexturenames; //mxd
		private readonly int leftboundary;
		private readonly int rightboundary;
		private readonly int topboundary;
		private readonly int bottomboundary;
		private readonly bool doomlightlevels;
		private readonly string actionspecialhelp; //mxd
		private readonly string thingclasshelp; //mxd
		
		// Skills
		private readonly List<SkillInfo> skills;

		// Map lumps
		private readonly Dictionary<string, MapLumpInfo> maplumps;

		//mxd. Map format
		private readonly bool doommapformat;
		private readonly bool hexenmapformat;
		private readonly bool universalmapformat;
		
		// Texture/flat/voxel sources
		private readonly IDictionary textureranges;
		private readonly IDictionary hiresranges; //mxd
		private readonly IDictionary flatranges;
		private readonly IDictionary patchranges;
		private readonly IDictionary spriteranges;
		private readonly IDictionary colormapranges;
		private readonly IDictionary voxelranges; //mxd
		
		// Things
		private readonly List<string> defaultthingflags;
		private readonly Dictionary<string, string> thingflags;
		private readonly List<ThingCategory> thingcategories;
		private readonly Dictionary<int, ThingTypeInfo> things;
		private readonly List<FlagTranslation> thingflagstranslation;
		private readonly Dictionary<string, ThingFlagsCompareGroup> thingflagscompare; //mxd 
		private readonly Dictionary<string, string> thingrenderstyles; //mxd
		
		// Linedefs
		private readonly Dictionary<string, string> linedefflags;
		private readonly List<string> sortedlinedefflags;
		private readonly Dictionary<int, LinedefActionInfo> linedefactions;
		private readonly List<LinedefActionInfo> sortedlinedefactions;
		private readonly List<LinedefActionCategory> actioncategories;
		private readonly List<LinedefActivateInfo> linedefactivates;
		private readonly List<GeneralizedCategory> genactioncategories;
		private readonly List<FlagTranslation> linedefflagstranslation;
		private readonly Dictionary<string, string> linedefrenderstyles; //mxd

		//mxd. Sidedefs
		private readonly Dictionary<string, string> sidedefflags; //mxd
		
		// Sectors
		private readonly Dictionary<string, string> sectorflags; //mxd
		private readonly Dictionary<int, SectorEffectInfo> sectoreffects;
		private readonly List<SectorEffectInfo> sortedsectoreffects;
		private readonly List<GeneralizedOption> geneffectoptions;
		private readonly StepsList brightnesslevels;
		private readonly Dictionary<string, string> sectorrenderstyles; //mxd

		// Universal fields
		private readonly List<UniversalFieldInfo> linedeffields;
		private readonly List<UniversalFieldInfo> sectorfields;
		private readonly List<UniversalFieldInfo> sidedeffields;
		private readonly List<UniversalFieldInfo> thingfields;
		private readonly List<UniversalFieldInfo> vertexfields;
		
		// Enums
		private readonly Dictionary<string, EnumList> enums;

		//mxd. DamageTypes
		private HashSet<string> damagetypes; 
		
		// Defaults
		private readonly List<DefinedTextureSet> texturesets;
		private readonly List<ThingsFilter> thingfilters;

		//mxd. Holds base game type (doom, heretic, hexen or strife)
		private readonly GameType gametype;
		
		#endregion

		#region ================== Properties

		// General settings
		public string Name { get { return configname; } }
		public string EngineName { get { return enginename; } }
		public string DefaultSaveCompiler { get { return defaultsavecompiler; } }
		public string DefaultTestCompiler { get { return defaulttestcompiler; } }
		public float DefaultTextureScale { get { return defaulttexturescale; } }
		public float DefaultFlatScale { get { return defaultflatscale; } }
		public string DefaultWallTexture { get { return defaultwalltexture; } } //mxd
		public string DefaultFloorTexture { get { return defaultfloortexture; } } //mxd
		public string DefaultCeilingTexture { get { return defaultceilingtexture; } } //mxd
		public bool ScaledTextureOffsets { get { return scaledtextureoffsets; } }
		public string FormatInterface { get { return formatinterface; } }
		public string DefaultLinedefActivationFlag { get { return defaultlinedefactivation; } } //mxd
		public string SingleSidedFlag { get { return singlesidedflag; } }
		public string DoubleSidedFlag { get { return doublesidedflag; } }
		public string ImpassableFlag { get { return impassableflag; } }
		public string UpperUnpeggedFlag { get { return upperunpeggedflag; } }
		public string LowerUnpeggedFlag { get { return lowerunpeggedflag; } }
		public bool MixTexturesFlats { get { return mixtexturesflats; } }
		public bool GeneralizedActions { get { return generalizedactions; } }
		public bool GeneralizedEffects { get { return generalizedeffects; } }
		public int Start3DModeThingType { get { return start3dmodethingtype; } }
		public int LinedefActivationsFilter { get { return linedefactivationsfilter; } }
		public string TestParameters { get { return testparameters; } }
		public bool TestShortPaths { get { return testshortpaths; } }
		public string MakeDoorTrack { get { return makedoortrack; } }
		public string MakeDoorDoor { get { return makedoordoor; } } //mxd
		public string MakeDoorCeiling { get { return makedoorceil; } } //mxd
		public int MakeDoorAction { get { return makedooraction; } }
		public int MakeDoorActivate { get { return makedooractivate; } }
		public Dictionary<string, bool> MakeDoorFlags { get { return makedoorflags; } }
		public int[] MakeDoorArgs { get { return makedoorargs; } }
		public bool LineTagIndicatesSectors { get { return linetagindicatesectors ; } }
		public string DecorateGames { get { return decorategames; } }
		public string SkyFlatName { get { return skyflatname; } internal set { skyflatname = value; } } //mxd. Added setter
		public Dictionary<string, string> DefaultSkyTextures { get { return defaultskytextures; } } //mxd
		public int MaxTextureNameLength { get { return maxtexturenamelength; } }
		public bool UseLongTextureNames { get { return longtexturenames; } } //mxd
		public int LeftBoundary { get { return leftboundary; } }
		public int RightBoundary { get { return rightboundary; } }
		public int TopBoundary { get { return topboundary; } }
		public int BottomBoundary { get { return bottomboundary; } }
		public bool DoomLightLevels { get { return doomlightlevels; } }
		public string ActionSpecialHelp { get { return actionspecialhelp; } } //mxd
		public string ThingClassHelp { get { return thingclasshelp; } } //mxd

		// Skills
		public List<SkillInfo> Skills { get { return skills; } }
		
		// Map lumps
		public Dictionary<string, MapLumpInfo> MapLumps { get { return maplumps; } }

		//mxd. Map format
		public bool UDMF { get { return universalmapformat; } }
		public bool HEXEN { get { return hexenmapformat; } }
		public bool DOOM { get { return doommapformat; } }

		// Texture/flat/voxel sources
		public IDictionary TextureRanges { get { return textureranges; } }
		public IDictionary HiResRanges { get { return hiresranges; } } //mxd
		public IDictionary FlatRanges { get { return flatranges; } }
		public IDictionary PatchRanges { get { return patchranges; } }
		public IDictionary SpriteRanges { get { return spriteranges; } }
		public IDictionary ColormapRanges { get { return colormapranges; } }
		public IDictionary VoxelRanges { get { return voxelranges; } } //mxd

		// Things
		public ICollection<string> DefaultThingFlags { get { return defaultthingflags; } }
		public IDictionary<string, string> ThingFlags { get { return thingflags; } }
		public List<FlagTranslation> ThingFlagsTranslation { get { return thingflagstranslation; } }
		public Dictionary<string, ThingFlagsCompareGroup> ThingFlagsCompare { get { return thingflagscompare; } } //mxd
		public Dictionary<string, string> ThingRenderStyles { get { return thingrenderstyles; } } //mxd
		
		// Linedefs
		public IDictionary<string, string> LinedefFlags { get { return linedefflags; } }
		public List<string> SortedLinedefFlags { get { return sortedlinedefflags; } }
		public IDictionary<int, LinedefActionInfo> LinedefActions { get { return linedefactions; } }
		public List<LinedefActionInfo> SortedLinedefActions { get { return sortedlinedefactions; } }
		public List<LinedefActionCategory> ActionCategories { get { return actioncategories; } }
		public List<LinedefActivateInfo> LinedefActivates { get { return linedefactivates; } }
		public List<GeneralizedCategory> GenActionCategories { get { return genactioncategories; } }
		public List<FlagTranslation> LinedefFlagsTranslation { get { return linedefflagstranslation; } }
		public Dictionary<string, string> LinedefRenderStyles { get { return linedefrenderstyles; } } //mxd

		//mxd. Sidedefs
		public IDictionary<string, string> SidedefFlags { get { return sidedefflags; } }

		// Sectors
		public IDictionary<string, string> SectorFlags { get { return sectorflags; } } //mxd
		public IDictionary<int, SectorEffectInfo> SectorEffects { get { return sectoreffects; } }
		public List<SectorEffectInfo> SortedSectorEffects { get { return sortedsectoreffects; } }
		public List<GeneralizedOption> GenEffectOptions { get { return geneffectoptions; } }
		public StepsList BrightnessLevels { get { return brightnesslevels; } }
		public Dictionary<string, string> SectorRenderStyles { get { return sectorrenderstyles; } } //mxd

		// Universal fields
		public List<UniversalFieldInfo> LinedefFields { get { return linedeffields; } }
		public List<UniversalFieldInfo> SectorFields { get { return sectorfields; } }
		public List<UniversalFieldInfo> SidedefFields { get { return sidedeffields; } }
		public List<UniversalFieldInfo> ThingFields { get { return thingfields; } }
		public List<UniversalFieldInfo> VertexFields { get { return vertexfields; } }

		// Enums
		public IDictionary<string, EnumList> Enums { get { return enums; } }

		//mxd. DamageTypes
		internal IEnumerable<string> DamageTypes { get { return damagetypes; } }

		// Defaults
		internal List<DefinedTextureSet> TextureSets { get { return texturesets; } }
		public List<ThingsFilter> ThingsFilters { get { return thingfilters; } }

		//mxd
		public GameType GameType { get { return gametype; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal GameConfiguration(Configuration cfg)
		{
			// Initialize
			this.cfg = cfg;
			this.thingflags = new Dictionary<string, string>(StringComparer.Ordinal);
			this.defaultthingflags = new List<string>();
			this.thingcategories = new List<ThingCategory>();
			this.things = new Dictionary<int, ThingTypeInfo>();
			this.linedefflags = new Dictionary<string, string>(StringComparer.Ordinal);
			this.sortedlinedefflags = new List<string>();
			this.linedefactions = new Dictionary<int, LinedefActionInfo>();
			this.actioncategories = new List<LinedefActionCategory>();
			this.sortedlinedefactions = new List<LinedefActionInfo>();
			this.linedefactivates = new List<LinedefActivateInfo>();
			this.sidedefflags = new Dictionary<string, string>(StringComparer.Ordinal); //mxd
			this.genactioncategories = new List<GeneralizedCategory>();
			this.sectorflags = new Dictionary<string, string>(StringComparer.Ordinal); //mxd
			this.sectoreffects = new Dictionary<int, SectorEffectInfo>();
			this.sortedsectoreffects = new List<SectorEffectInfo>();
			this.geneffectoptions = new List<GeneralizedOption>();
			this.enums = new Dictionary<string, EnumList>(StringComparer.Ordinal);
			this.skills = new List<SkillInfo>();
			this.texturesets = new List<DefinedTextureSet>();
			this.makedoorargs = new int[Linedef.NUM_ARGS];
			this.maplumps = new Dictionary<string, MapLumpInfo>(StringComparer.Ordinal);
			this.thingflagstranslation = new List<FlagTranslation>();
			this.linedefflagstranslation = new List<FlagTranslation>();
			this.thingfilters = new List<ThingsFilter>();
			this.thingflagscompare = new Dictionary<string, ThingFlagsCompareGroup>(); //mxd
			this.brightnesslevels = new StepsList();
			this.makedoorflags = new Dictionary<string, bool>(StringComparer.Ordinal);
			this.linedefrenderstyles = new Dictionary<string, string>(StringComparer.Ordinal); //mxd
			this.sectorrenderstyles = new Dictionary<string, string>(StringComparer.Ordinal); //mxd
			this.thingrenderstyles = new Dictionary<string, string>(StringComparer.Ordinal); //mxd
			this.defaultskytextures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); //mxd
			
			// Read general settings
			configname = cfg.ReadSetting("game", "<unnamed game>");

			//mxd
			int gt = (cfg.ReadSetting("basegame", (int)GameType.UNKNOWN));
			gametype = ( (gt > -1 && gt < Gldefs.GLDEFS_LUMPS_PER_GAME.Length) ? (GameType)gt : GameType.UNKNOWN);

			enginename = cfg.ReadSetting("engine", "");
			defaultsavecompiler = cfg.ReadSetting("defaultsavecompiler", "");
			defaulttestcompiler = cfg.ReadSetting("defaulttestcompiler", "");
			defaulttexturescale = cfg.ReadSetting("defaulttexturescale", 1f);
			defaultflatscale = cfg.ReadSetting("defaultflatscale", 1f);
			defaultwalltexture = cfg.ReadSetting("defaultwalltexture", "STARTAN"); //mxd
			defaultfloortexture = cfg.ReadSetting("defaultfloortexture", "FLOOR0_1"); //mxd
			defaultceilingtexture = cfg.ReadSetting("defaultceilingtexture", "CEIL1_1"); //mxd
			scaledtextureoffsets = cfg.ReadSetting("scaledtextureoffsets", true);
			formatinterface = cfg.ReadSetting("formatinterface", "");
			mixtexturesflats = cfg.ReadSetting("mixtexturesflats", false);
			generalizedactions = cfg.ReadSetting("generalizedlinedefs", false);
			generalizedeffects = cfg.ReadSetting("generalizedsectors", false);
			start3dmodethingtype = cfg.ReadSetting("start3dmode", 0);
			linedefactivationsfilter = cfg.ReadSetting("linedefactivationsfilter", 0);
			testparameters = cfg.ReadSetting("testparameters", "");
			testshortpaths = cfg.ReadSetting("testshortpaths", false);
			makedoortrack = cfg.ReadSetting("makedoortrack", "-");
			makedoordoor = cfg.ReadSetting("makedoordoor", "-"); //mxd
			makedoorceil = cfg.ReadSetting("makedoorceil", "-"); //mxd
			makedooraction = cfg.ReadSetting("makedooraction", 0);
			makedooractivate = cfg.ReadSetting("makedooractivate", 0);
			linetagindicatesectors = cfg.ReadSetting("linetagindicatesectors", false);
			decorategames = cfg.ReadSetting("decorategames", "");
			skyflatname = cfg.ReadSetting("skyflatname", "F_SKY1");
			leftboundary = cfg.ReadSetting("leftboundary", -32768);
			rightboundary = cfg.ReadSetting("rightboundary", 32767);
			topboundary = cfg.ReadSetting("topboundary", 32767);
			bottomboundary = cfg.ReadSetting("bottomboundary", -32768);
			doomlightlevels = cfg.ReadSetting("doomlightlevels", true);
			actionspecialhelp = cfg.ReadSetting("actionspecialhelp", string.Empty); //mxd
			thingclasshelp = cfg.ReadSetting("thingclasshelp", string.Empty); //mxd
			defaultlinedefactivation = cfg.ReadSetting("defaultlinedefactivation", ""); //mxd
			for(int i = 0; i < Linedef.NUM_ARGS; i++) makedoorargs[i] = cfg.ReadSetting("makedoorarg" + i.ToString(CultureInfo.InvariantCulture), 0);

			//mxd. Update map format flags
			universalmapformat = (formatinterface == "UniversalMapSetIO");
			hexenmapformat = (formatinterface == "HexenMapSetIO");
			doommapformat = (formatinterface == "DoomMapSetIO");

			//mxd. Texture names length
			longtexturenames = cfg.ReadSetting("longtexturenames", false);
			maxtexturenamelength = (longtexturenames ? short.MaxValue : DataManager.CLASIC_IMAGE_NAME_LENGTH);

			// Flags have special (invariant culture) conversion
			// because they are allowed to be written as integers in the configs
			object obj = cfg.ReadSettingObject("singlesidedflag", 0);
			if(obj is int) singlesidedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else singlesidedflag = obj.ToString();
			obj = cfg.ReadSettingObject("doublesidedflag", 0);
			if(obj is int) doublesidedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else doublesidedflag = obj.ToString();
			obj = cfg.ReadSettingObject("impassableflag", 0);
			if(obj is int) impassableflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else impassableflag = obj.ToString();
			obj = cfg.ReadSettingObject("upperunpeggedflag", 0);
			if(obj is int) upperunpeggedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else upperunpeggedflag = obj.ToString();
			obj = cfg.ReadSettingObject("lowerunpeggedflag", 0);
			if(obj is int) lowerunpeggedflag = ((int)obj).ToString(CultureInfo.InvariantCulture); else lowerunpeggedflag = obj.ToString();

			// Get texture and flat sources
			textureranges = cfg.ReadSetting("textures", new Hashtable());
			hiresranges = cfg.ReadSetting("hires", new Hashtable()); //mxd
			flatranges = cfg.ReadSetting("flats", new Hashtable());
			patchranges = cfg.ReadSetting("patches", new Hashtable());
			spriteranges = cfg.ReadSetting("sprites", new Hashtable());
			colormapranges = cfg.ReadSetting("colormaps", new Hashtable());
			voxelranges = cfg.ReadSetting("voxels", new Hashtable()); //mxd
			
			// Map lumps
			LoadMapLumps();
			
			// Skills
			LoadSkills();

			// Enums
			LoadEnums();

			//mxd. Damage types
			LoadDamageTypes();
			
			// Things
			LoadThingFlags();
			LoadDefaultThingFlags();
			LoadThingCategories();
			LoadStringDictionary(thingrenderstyles, "thingrenderstyles"); //mxd
			
			// Linedefs
			LoadLinedefFlags();
			LoadLinedefActions();
			LoadLinedefActivations();
			LoadLinedefGeneralizedActions();
			LoadStringDictionary(linedefrenderstyles, "linedefrenderstyles"); //mxd

			//mxd. Sidedefs
			LoadStringDictionary(sidedefflags, "sidedefflags");

			// Sectors
			LoadStringDictionary(sectorflags, "sectorflags"); //mxd
			LoadBrightnessLevels();
			LoadSectorEffects();
			LoadSectorGeneralizedEffects();
			LoadStringDictionary(sectorrenderstyles, "sectorrenderstyles"); //mxd
			
			// Universal fields
			linedeffields = LoadUniversalFields("linedef");
			sectorfields = LoadUniversalFields("sector");
			sidedeffields = LoadUniversalFields("sidedef");
			thingfields = LoadUniversalFields("thing");
			vertexfields = LoadUniversalFields("vertex");

			// Defaults
			LoadTextureSets();
			LoadThingFilters();

			//mxd. Vanilla sky textures
			LoadDefaultSkies();

			// Make door flags
			LoadMakeDoorFlags();
		}

		// Destructor
		~GameConfiguration()
		{
			foreach(ThingCategory tc in thingcategories) tc.Dispose();
			foreach(LinedefActionCategory ac in actioncategories) ac.Dispose();
			foreach(ThingsFilter tf in thingfilters) tf.Dispose(); //mxd
			foreach(GeneralizedCategory gc in genactioncategories) gc.Dispose(); //mxd
		}
		
		#endregion

		#region ================== Loading
		
		// This loads the map lumps
		private void LoadMapLumps() 
		{
			// Get map lumps list
			IDictionary dic = cfg.ReadSetting("maplumpnames", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make map lumps
				MapLumpInfo lumpinfo = new MapLumpInfo(de.Key.ToString(), cfg);
				maplumps.Add(de.Key.ToString(), lumpinfo);
			}
		}
		
		// This loads the enumerations
		private void LoadEnums() 
		{
			// Get enums list
			IDictionary dic = cfg.ReadSetting("enums", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make new enum
				EnumList list = new EnumList(de.Key.ToString(), cfg);
				enums.Add(de.Key.ToString(), list);
			}
		}

		//mxd. This loads built-in DamageTypes
		private void LoadDamageTypes()
		{
			string dtypes = cfg.ReadSetting("damagetypes", "None");
			string[] dtypesarr = dtypes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			damagetypes = new HashSet<string>();

			foreach(string dtype in dtypesarr)
			{
				if(damagetypes.Contains(dtype))
				{
					General.ErrorLogger.Add(ErrorType.Warning, "DamageType \"" + dtype + "\" is double defined in the \"" + this.Name + "\" game configuration");
					continue;
				}

				damagetypes.Add(dtype);
			}
		}
		
		// This loads a universal fields list
		private List<UniversalFieldInfo> LoadUniversalFields(string elementname)
		{
			List<UniversalFieldInfo> list = new List<UniversalFieldInfo>();

			// Get fields
			IDictionary dic = cfg.ReadSetting("universalfields." + elementname, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				try
				{
					// Read the field info and add to list
					UniversalFieldInfo uf = new UniversalFieldInfo(elementname, de.Key.ToString(), cfg, enums);
					list.Add(uf);
				}
				catch(Exception)
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Unable to read universal field definition \"universalfields." + elementname + "." + de.Key + "\" from game configuration \"" + this.Name + "\"");
				}
			}

			// Return result
			return list;
		}
		
		// Things and thing categories
		private void LoadThingCategories()
		{
			// Get thing categories
			IDictionary dic = cfg.ReadSetting("thingtypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				if(de.Value is IDictionary)
				{
					// Make a category
					ThingCategory thingcat = new ThingCategory(cfg, null, de.Key.ToString(), enums);

					//mxd. Otherwise nesting problems might occure
					if(thingcat.IsValid)
					{
						// Add all things in category to the big list
						AddThingsFromCategory(thingcat); //mxd

						// Add category to list
						thingcategories.Add(thingcat);
					}
				}
			}
		}

		//mxd. This recursively adds all things from a ThingCategory and it's children
		private void AddThingsFromCategory(ThingCategory thingcat) 
		{
			if(!thingcat.IsValid) return;
			
			// Add all things in category to the big list
			foreach(ThingTypeInfo t in thingcat.Things) 
			{
				if(!things.ContainsKey(t.Index)) 
					things.Add(t.Index, t);
				else
					General.ErrorLogger.Add(ErrorType.Warning, "Thing number " + t.Index + " is defined more than once (as \"" + things[t.Index].Title + "\" and \"" + t.Title + "\") in the \"" + this.Name + "\" game configuration");
			}

			// Recursively add things from child categories
			foreach(ThingCategory c in thingcat.Children) AddThingsFromCategory(c);
		}
		
		// Linedef flags
		private void LoadLinedefFlags()
		{
			// Get linedef flags
			LoadStringDictionary(linedefflags, "linedefflags"); //mxd
			
			// Get translations
			IDictionary dic = cfg.ReadSetting("linedefflagstranslation", new Hashtable());
			foreach(DictionaryEntry de in dic)
				linedefflagstranslation.Add(new FlagTranslation(de));
			
			// Sort flags?
			MapSetIO io = MapSetIO.Create(formatinterface);
			if(io.HasNumericLinedefFlags)
			{
				// Make list for integers that we can sort
				List<int> sortlist = new List<int>(linedefflags.Count);
				foreach(KeyValuePair<string, string> f in linedefflags)
				{
					int num;
					if(int.TryParse(f.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out num)) sortlist.Add(num);
				}
				
				// Sort
				sortlist.Sort();
				
				// Make list of strings
				foreach(int i in sortlist)
					sortedlinedefflags.Add(i.ToString(CultureInfo.InvariantCulture));
			}
			
			// Sort the flags, because they must be compared highest first!
			linedefflagstranslation.Sort();
		}

		// Linedef actions and action categories
		private void LoadLinedefActions()
		{
			Dictionary<string, LinedefActionCategory> cats = new Dictionary<string, LinedefActionCategory>(StringComparer.Ordinal);

			// Get linedef categories
			IDictionary dic = cfg.ReadSetting("linedeftypes", new Hashtable());
			foreach(DictionaryEntry cde in dic)
			{
				if(cde.Value is IDictionary)
				{
					// Read category title
					string cattitle = cfg.ReadSetting("linedeftypes." + cde.Key + ".title", "");

					// Make or get category
					LinedefActionCategory ac;
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
						int actionnumber;
						if(int.TryParse(de.Key.ToString(),
							NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
							CultureInfo.InvariantCulture, out actionnumber))
						{
							// Check if the item value is a structure
							if(de.Value is IDictionary)
							{
								// Make the line type
								LinedefActionInfo ai = new LinedefActionInfo(actionnumber, cfg, cde.Key.ToString(), enums);

								// Add action to category and sorted list
								sortedlinedefactions.Add(ai);
								linedefactions.Add(actionnumber, ai);
								ac.Add(ai);
							}
							else
							{
								// Failure
								if(de.Value != null)
									General.ErrorLogger.Add(ErrorType.Warning, "Structure \"linedeftypes\" contains invalid types in the \"" + this.Name + "\" game configuration. All types must be expanded structures.");
							}
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
			// Get linedef activations
			IDictionary dic = cfg.ReadSetting("linedefactivations", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Add to the list
				linedefactivates.Add(new LinedefActivateInfo(de.Key.ToString(), de.Value.ToString()));
			}

			//mxd. Sort only when activations are numeric
			MapSetIO io = MapSetIO.Create(formatinterface);
			if(io.HasNumericLinedefActivations)
			{
				linedefactivates.Sort();
			}
		}

		// Linedef generalized actions
		private void LoadLinedefGeneralizedActions()
		{
			// Get linedef activations
			IDictionary dic = cfg.ReadSetting("gen_linedeftypes", new Hashtable());
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
					General.ErrorLogger.Add(ErrorType.Warning, "Structure \"gen_linedeftypes\" contains invalid entries in the \"" + this.Name + "\" game configuration");
				}
			}
		}

		// Sector effects
		private void LoadSectorEffects()
		{
			// Get sector effects
			IDictionary dic = cfg.ReadSetting("sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try parsing the action number
				int actionnumber;
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out actionnumber))
				{
					// Make effects
					SectorEffectInfo si = new SectorEffectInfo(actionnumber, de.Value.ToString(), true, false);
					
					// Add action to category and sorted list
					sortedsectoreffects.Add(si);
					sectoreffects.Add(actionnumber, si);
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure \"sectortypes\" contains invalid keys in the \"" + this.Name + "\" game configuration");
				}
			}

			// Sort the actions list
			sortedsectoreffects.Sort();
		}

		// Brightness levels
		private void LoadBrightnessLevels()
		{
			// Get brightness levels structure
			IDictionary dic = cfg.ReadSetting("sectorbrightness", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the level
				int level;
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out level))
				{
					brightnesslevels.Add(level);
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure \"sectorbrightness\" contains invalid keys in the \"" + this.Name + "\" game configuration");
				}
			}

			// Sort the list
			brightnesslevels.Sort();
		}

		// Sector generalized effects
		private void LoadSectorGeneralizedEffects()
		{
			// Get sector effects
			IDictionary dic = cfg.ReadSetting("gen_sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				IDictionary value = de.Value as IDictionary;
				if(value != null)
				{
					// Add option
					geneffectoptions.Add(new GeneralizedOption("gen_sectortypes", "", de.Key.ToString(), value));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure \"gen_sectortypes\" contains invalid entries in the \"" + this.Name + "\" game configuration");
				}
			}
		}

		// Thing flags
		private void LoadThingFlags()
		{
			// Get thing flags
			LoadStringDictionary(thingflags, "thingflags"); //mxd
			
			// Get translations
			IDictionary dic = cfg.ReadSetting("thingflagstranslation", new Hashtable());
			foreach(DictionaryEntry de in dic)
				thingflagstranslation.Add(new FlagTranslation(de));
				
			// Get thing compare flag info (for the stuck thing error checker
			HashSet<string> flagscache = new HashSet<string>();
			dic = cfg.ReadSetting("thingflagscompare", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				string group = de.Key.ToString(); //mxd
				thingflagscompare[group] = new ThingFlagsCompareGroup(cfg, group); //mxd
				foreach(string s in thingflagscompare[group].Flags.Keys)
				{
					if(flagscache.Contains(s))
						General.ErrorLogger.Add(ErrorType.Warning, "ThingFlagsCompare flag \"" + s + "\" is double defined in the \"" + group + "\" group of the \"" + this.Name + "\" game configuration");
					else
						flagscache.Add(s);
				}
			}

			//mxd. Integrity check
			foreach(KeyValuePair<string, ThingFlagsCompareGroup> group in thingflagscompare)
			{
				foreach(ThingFlagsCompare flag in group.Value.Flags.Values)
				{
					// Required groups are missing?
					foreach(string s in flag.RequiredGroups)
					{
						if(!thingflagscompare.ContainsKey(s))
						{
							General.ErrorLogger.Add(ErrorType.Warning, "ThingFlagsCompare group \"" + s + "\" required by flag \"" + flag.Flag + "\" does not exist in the \"" + this.Name + "\" game configuration");
							flag.RequiredGroups.Remove(s);
						}
					}

					// Ignored groups are missing?
					foreach(string s in flag.IgnoredGroups)
					{
						if(!thingflagscompare.ContainsKey(s))
						{
							General.ErrorLogger.Add(ErrorType.Warning, "ThingFlagsCompare group \"" + s + "\", ignored by flag \"" + flag.Flag + "\" does not exist in the \"" + this.Name + "\" game configuration");
							flag.IgnoredGroups.Remove(s);
						}
					}

					// Required flag is missing?
					if(!string.IsNullOrEmpty(flag.RequiredFlag) && !flagscache.Contains(flag.RequiredFlag)) 
					{
						General.ErrorLogger.Add(ErrorType.Warning, "ThingFlagsCompare flag \"" + flag.RequiredFlag + "\", required by flag \"" + flag.Flag + "\" does not exist in the \"" + this.Name + "\" game configuration");
						flag.RequiredFlag = string.Empty;
					}
				}
			}

			// Sort the translation flags, because they must be compared highest first!
			thingflagstranslation.Sort();
		}

		// Default thing flags
		private void LoadDefaultThingFlags()
		{
			// Get linedef flags
			IDictionary dic = cfg.ReadSetting("defaultthingflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if flag exists
				if(thingflags.ContainsKey(de.Key.ToString()))
				{
					defaultthingflags.Add(de.Key.ToString());
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure \"defaultthingflags\" contains unknown thing flags in the \"" + this.Name + "\" game configuration");
				}
			}
		}

		// Skills
		private void LoadSkills()
		{
			// Get skills
			IDictionary dic = cfg.ReadSetting("skills", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				int num;
				if(int.TryParse(de.Key.ToString(), out num))
				{
					skills.Add(new SkillInfo(num, de.Value.ToString()));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure \"skills\" contains invalid skill numbers in the \"" + this.Name + "\" game configuration");
				}
			}
		}
		
		// Texture Sets
		private void LoadTextureSets() 
		{
			// Get sets
			IDictionary dic = cfg.ReadSetting("texturesets", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				DefinedTextureSet s = new DefinedTextureSet(cfg, "texturesets." + de.Key);
				texturesets.Add(s);
			}
		}
		
		// Thing Filters
		private void LoadThingFilters()
		{
			// Get sets
			IDictionary dic = cfg.ReadSetting("thingsfilters", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				ThingsFilter f = new ThingsFilter(cfg, "thingsfilters." + de.Key);
				thingfilters.Add(f);
			}
		}

		// Make door flags
		private void LoadMakeDoorFlags()
		{
			IDictionary dic = cfg.ReadSetting("makedoorflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Using minus will unset the flag
				if(de.Key.ToString()[0] == '-')
				{
					makedoorflags[de.Key.ToString().TrimStart('-')] = false;
				}
				else
				{
					makedoorflags[de.Key.ToString()] = true;
				}
			}
		}

		//mxd
		private void LoadDefaultSkies()
		{
			IDictionary dic = cfg.ReadSetting("defaultskytextures", new Hashtable());
			char[] separator = new []{ ',' };
			foreach(DictionaryEntry de in dic)
			{
				string skytex = de.Key.ToString();
				if(defaultskytextures.ContainsKey(skytex))
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Sky texture \"" + skytex + "\" is double defined in the \"" + this.Name + "\" game configuration");
					continue;
				}

				string[] maps = de.Value.ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if(maps.Length == 0)
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Sky texture \"" + skytex + "\" has no map names defined in the \"" + this.Name + "\" game configuration");
					continue;
				}

				foreach(string map in maps)
				{
					if(defaultskytextures.ContainsKey(map))
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Map \"" + map + "\" is double defined in the \"DefaultSkyTextures\" block of \"" + this.Name + "\" game configuration");
						continue;
					}

					defaultskytextures[map] = skytex;
				}
			}
		}

		//mxd
		private void LoadStringDictionary(Dictionary<string, string> target, string settingname) 
		{
			IDictionary dic = cfg.ReadSetting(settingname, new Hashtable());
			foreach(DictionaryEntry de in dic)
				target.Add(de.Key.ToString(), de.Value.ToString());
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
		
		// This gets a list of things categories
		internal List<ThingCategory> GetThingCategories()
		{
			return new List<ThingCategory>(thingcategories);
		}
		
		// This gets a list of things
		internal Dictionary<int, ThingTypeInfo> GetThingTypes()
		{
			return new Dictionary<int, ThingTypeInfo>(things);
		}
		
		// This checks if an action is generalized or predefined
		public static bool IsGeneralized(int action) { return IsGeneralized(action, General.Map.Config.GenActionCategories); }
		public static bool IsGeneralized(int action, IEnumerable<GeneralizedCategory> categories)
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

		//mxd
		public static bool IsGeneralizedSectorEffect(int effect, List<GeneralizedOption> options) 
		{
			if(effect == 0) return false;

			int cureffect = effect;
			for(int i = options.Count - 1; i > -1; i--)
			{
				for(int j = options[i].Bits.Count - 1; j > -1; j--)
				{
					GeneralizedBit bit = options[i].Bits[j];
					if(bit.Index > 0 && (cureffect & bit.Index) == bit.Index) return true;
					cureffect -= bit.Index;
				}
			}

			return false;
		}

		//mxd
		public static HashSet<int> GetGeneralizedSectorEffectBits(int effect) { return GetGeneralizedSectorEffectBits(effect, General.Map.Config.GenEffectOptions); }
		public static HashSet<int> GetGeneralizedSectorEffectBits(int effect, List<GeneralizedOption> options)
		{
			HashSet<int> result = new HashSet<int>();
			if(effect > 0)
			{
				int cureffect = effect;
				for(int i = options.Count - 1; i > -1; i--)
				{
					for(int j = options[i].Bits.Count - 1; j > -1; j--)
					{
						GeneralizedBit bit = options[i].Bits[j];
						if(bit.Index > 0 && (cureffect & bit.Index) == bit.Index)
						{
							cureffect -= bit.Index;
							result.Add(bit.Index);
						}
					}
				}

				if(cureffect > 0) result.Add(cureffect);
			}

			return result;
		}

		//mxd
		public string GetGeneralizedSectorEffectName(int effect) 
		{
			if(effect == 0) return "None";
			string title = "Unknown generalized effect";
			int matches = 0;

			int nongeneralizedeffect = effect;

			// Check all options, in bigger to smaller order
			for(int i = geneffectoptions.Count - 1; i > -1; i--)
			{
				for(int j = geneffectoptions[i].Bits.Count - 1; j > -1; j--)
				{
					GeneralizedBit bit = geneffectoptions[i].Bits[j];
					if(bit.Index > 0 && (effect & bit.Index) == bit.Index)
					{
						title = geneffectoptions[i].Name + ": " + bit.Title;
						nongeneralizedeffect -= bit.Index;
						matches++;
						break;
					}
				}
			}

			// Make generalized effect title
			string gentitle = (matches > 1 ? "Generalized (" + matches + " effects)" : title);
			
			// Generalized effect only
			if(nongeneralizedeffect <= 0) return gentitle;

			// Classic and generalized effects
			if(General.Map.Config.SectorEffects.ContainsKey(nongeneralizedeffect))
				return General.Map.Config.SectorEffects[nongeneralizedeffect].Title + " + " + gentitle;

			if(matches > 0) return "Unknown effect + " + gentitle;
			return "Unknown effect";
		}
		
		// This checks if a specific edit mode class is listed
		public bool IsEditModeSpecified(string classname)
		{
			return cfg.SettingExists("editingmodes." + classname.ToString(CultureInfo.InvariantCulture));
		}
		
		// This returns information on a linedef type
		public LinedefActionInfo GetLinedefActionInfo(int action)
		{
			// No action?
			if(action == 0) return new LinedefActionInfo(0, "None", true, false);
			
			// Known type?
			if(linedefactions.ContainsKey(action)) return linedefactions[action];

			// Generalized action?
			if(IsGeneralized(action, genactioncategories))
				return new LinedefActionInfo(action, "Generalized (" + GetGeneralizedActionCategory(action) + ")", true, true);

			// Unknown action...
			return new LinedefActionInfo(action, "Unknown", false, false);
		}

		// This returns information on a sector effect
		public SectorEffectInfo GetSectorEffectInfo(int effect)
		{
			// No effect?
			if(effect == 0) return new SectorEffectInfo(0, "None", true, false);
			
			// Known type?
			if(sectoreffects.ContainsKey(effect)) return sectoreffects[effect];
	
			//mxd. Generalized sector effect?
			if(IsGeneralizedSectorEffect(effect, geneffectoptions))
				return new SectorEffectInfo(effect, GetGeneralizedSectorEffectName(effect), true, true);

			// Unknown sector effect...
			return new SectorEffectInfo(effect, "Unknown", false, false);
		}
		
		#endregion
	}
}
