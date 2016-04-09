
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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public struct SpriteFrameInfo //mxd
	{
		public string Sprite;
		public long SpriteLongName;
		public bool Mirror;
	}
	
	public class ThingTypeInfo : INumberedTitle, IComparable<ThingTypeInfo>
	{
		#region ================== Constants

		public const int THING_BLOCKING_NONE = 0;
		public const int THING_BLOCKING_FULL = 1;
		public const int THING_BLOCKING_HEIGHT = 2;
		public const int THING_ERROR_NONE = 0;
		public const int THING_ERROR_INSIDE = 1;
		public const int THING_ERROR_INSIDE_STUCK = 2;
		private const float THING_FIXED_SIZE = 14f; //mxd
		
		#endregion

		#region ================== Variables

		// Properties
		private readonly int index;
		private string title;
		private string sprite;
		private SpriteFrameInfo[] spriteframe; //mxd. All rotations for given sprite. Currently contains either 1 or 8 frames
		private ActorStructure actor;
		private string classname; //mxd
		private int color;
		private float alpha; //mxd
		private byte alphabyte; //mxd
		private string renderstyle; //mxd
		private bool bright; //mxd
		private bool arrow;
		private float radius;
		private float height;
		private bool hangs;
		private int blocking;
		private int errorcheck;
		private readonly bool fixedsize;
		private readonly bool fixedrotation; //mxd
		private readonly ThingCategory category;
		private readonly ArgumentInfo[] args;
		private readonly bool isknown;
		private readonly bool absolutez;
		private bool xybillboard; //mxd
		private SizeF spritescale;
		private readonly bool locksprite; //mxd
		private bool obsolete; //mxd
		private string obsoletemessage; //mxd

		//mxd. GLOOME rendering settings
		private Thing.SpriteRenderMode rendermode;
		private bool rollsprite;
		private bool sticktoplane;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Title { get { return title; } }
		public string Sprite { get { return sprite; } }
		public SpriteFrameInfo[] SpriteFrame { get { return spriteframe; } }
		public ActorStructure Actor { get { return actor; } }
		public int Color { get { return color; } }
		public float Alpha { get { return alpha; } } //mxd
		public byte AlphaByte { get { return alphabyte; } } //mxd
		public string RenderStyle { get { return renderstyle; } } //mxd
		public bool Bright { get { return bright; } } //mxd
		public bool Arrow { get { return arrow; } }
		public float Radius { get { return radius; } }
		public float Height { get { return height; } }
		public bool Hangs { get { return hangs; } }
		public int Blocking { get { return blocking; } }
		public int ErrorCheck { get { return errorcheck; } }
		public bool FixedSize { get { return fixedsize; } }
		public bool FixedRotation { get { return fixedrotation; } } //mxd
		public ThingCategory Category { get { return category; } }
		public ArgumentInfo[] Args { get { return args; } }
		public bool IsKnown { get { return isknown; } }
		public bool IsNull { get { return (index == 0); } }
		public bool IsObsolete { get { return obsolete; } } //mxd
		public string ObsoleteMessage { get { return obsoletemessage; } } //mxd
		public bool AbsoluteZ { get { return absolutez; } }
		public bool XYBillboard { get { return xybillboard; } } //mxd
		public SizeF SpriteScale { get { return spritescale; } }
		public string ClassName { get { return classname; } } //mxd. Need this to add model overrides for things defined in configs

		//mxd. GLOOME rendering flags
		public Thing.SpriteRenderMode RenderMode { get { return rendermode; } }
		public bool RollSprite { get { return rollsprite; } }
		public bool StickToPlane { get { return sticktoplane; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ThingTypeInfo(int index)
		{
			// Initialize
			this.index = index;
			this.category = null;
			this.actor = null;
			this.title = "<" + index.ToString(CultureInfo.InvariantCulture) + ">";
			this.sprite = DataManager.INTERNAL_PREFIX + "unknownthing";
			this.classname = string.Empty; //mxd
			this.color = 0;
			this.alpha = 1f; //mxd
			this.alphabyte = 255; //mxd
			this.renderstyle = "normal"; //mxd
			this.bright = false; //mxd
			this.arrow = true;
			this.radius = 10f;
			this.height = 20f;
			this.hangs = false;
			this.blocking = 0;
			this.errorcheck = 0;
			this.spritescale = new SizeF(1.0f, 1.0f);
			this.fixedsize = false;
			this.fixedrotation = false; //mxd
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true) } }; //mxd
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			this.isknown = false;
			this.absolutez = false;
			this.xybillboard = false;
			this.locksprite = false; //mxd
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		internal ThingTypeInfo(ThingCategory cat, int index, Configuration cfg, IDictionary<string, EnumList> enums)
		{
			string key = index.ToString(CultureInfo.InvariantCulture);
			
			// Initialize
			this.index = index;
			this.category = cat;
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			this.isknown = true;
			this.actor = null;
			this.bright = false; //mxd
		
			// Read properties
			this.title = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".title", "<" + key + ">");
			this.sprite = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".sprite", cat.Sprite);
			this.color = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".color", cat.Color);
			this.alpha = General.Clamp(cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".alpha", cat.Alpha), 0f, 1f); //mxd
			this.alphabyte = (byte)(this.alpha * 255); //mxd
			this.renderstyle = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".renderstyle", cat.RenderStyle).ToLower(); //mxd
			this.arrow = (cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".arrow", cat.Arrow) != 0);
			this.radius = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".width", cat.Radius);
			this.height = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".height", cat.Height);
			this.hangs = (cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".hangs", cat.Hangs) != 0);
			this.blocking = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".blocking", cat.Blocking);
			this.errorcheck = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".error", cat.ErrorCheck);
			this.fixedsize = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".fixedsize", cat.FixedSize);
			this.fixedrotation = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".fixedrotation", cat.FixedRotation); //mxd
			this.absolutez = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".absolutez", cat.AbsoluteZ);
			float sscale = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".spritescale", cat.SpriteScale);
			this.spritescale = new SizeF(sscale, sscale);
			this.locksprite = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".locksprite", false); //mxd
			this.classname = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".class", String.Empty); //mxd
			
			// Read the args
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
				this.args[i] = new ArgumentInfo(cfg, "thingtypes." + cat.Name + "." + key, i, enums);
			
			// Safety
			if(this.radius < 4f || this.fixedsize) this.radius = THING_FIXED_SIZE;
			if(this.hangs && this.absolutez) this.hangs = false; //mxd

			//mxd. Create sprite frame
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true) } };
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public ThingTypeInfo(ThingCategory cat, int index, string title)
		{
			// Initialize
			this.index = index;
			this.category = cat;
			this.title = title;
			this.actor = null;
			this.classname = string.Empty; //mxd
			this.isknown = true;
			this.bright = false; //mxd
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++) this.args[i] = new ArgumentInfo(i);
			
			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
			this.arrow = (cat.Arrow != 0);
			this.alpha = cat.Alpha; //mxd
			this.alphabyte = (byte)(this.alpha * 255); //mxd
			this.renderstyle = cat.RenderStyle; //mxd
			this.radius = cat.Radius;
			this.height = cat.Height;
			this.hangs = (cat.Hangs != 0);
			this.blocking = cat.Blocking;
			this.errorcheck = cat.ErrorCheck;
			this.fixedsize = cat.FixedSize;
			this.fixedrotation = cat.FixedRotation; //mxd
			this.absolutez = cat.AbsoluteZ;
			this.spritescale = new SizeF(cat.SpriteScale, cat.SpriteScale);
			this.locksprite = false;

			// Safety
			if(this.radius < 4f || this.fixedsize) this.radius = THING_FIXED_SIZE;
			if(this.hangs && this.absolutez) this.hangs = false; //mxd

			//mxd. Create sprite frame
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true) } };

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		internal ThingTypeInfo(ThingCategory cat, ActorStructure actor)
		{
			// Initialize
			this.index = actor.DoomEdNum;
			this.category = cat;
			this.title = "";
			this.actor = actor;
			this.classname = actor.ClassName; //mxd
			this.isknown = true;
			this.bright = false; //mxd
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++) this.args[i] = new ArgumentInfo(i);
			
			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
			this.alpha = cat.Alpha; //mxd
			this.alphabyte = (byte)(this.alpha * 255); //mxd
			this.renderstyle = cat.RenderStyle; //mxd
			this.arrow = (cat.Arrow != 0);
			this.radius = cat.Radius;
			this.height = cat.Height;
			this.hangs = (cat.Hangs != 0);
			this.blocking = cat.Blocking;
			this.errorcheck = cat.ErrorCheck;
			this.fixedsize = cat.FixedSize;
			this.fixedrotation = cat.FixedRotation; //mxd
			this.absolutez = cat.AbsoluteZ;
			this.spritescale = new SizeF(cat.SpriteScale, cat.SpriteScale);

			// Safety
			if(this.hangs && this.absolutez) this.hangs = false; //mxd
			
			// Apply settings from actor
			ModifyByDecorateActor(actor);

			//mxd. Create sprite frame
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true) } };
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		//mxd. Constructor
		internal ThingTypeInfo(ThingCategory cat, ActorStructure actor, int index)
		{
			// Initialize
			this.index = index;
			this.category = cat;
			this.title = "";
			this.actor = actor;
			this.classname = actor.ClassName; //mxd
			this.isknown = true;
			this.bright = false; //mxd
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++) this.args[i] = new ArgumentInfo(i);

			// Read properties
			this.sprite = cat.Sprite;
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true), } }; //mxd
			this.color = cat.Color;
			this.alpha = cat.Alpha; //mxd
			this.alphabyte = (byte)(this.alpha * 255); //mxd
			this.renderstyle = cat.RenderStyle; //mxd
			this.arrow = (cat.Arrow != 0);
			this.radius = cat.Radius;
			this.height = cat.Height;
			this.hangs = (cat.Hangs != 0);
			this.blocking = cat.Blocking;
			this.errorcheck = cat.ErrorCheck;
			this.fixedsize = cat.FixedSize;
			this.fixedrotation = cat.FixedRotation; //mxd
			this.absolutez = cat.AbsoluteZ;
			this.spritescale = new SizeF(cat.SpriteScale, cat.SpriteScale);

			// Safety
			if(this.hangs && this.absolutez) this.hangs = false; //mxd

			// Apply settings from actor
			ModifyByDecorateActor(actor);

			//mxd. Create sprite frame
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true) } };

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		internal ThingTypeInfo(int index, ThingTypeInfo other) 
		{
			// Initialize
			this.index = index;
			this.category = other.category;
			this.title = other.title;
			this.actor = other.actor;
			this.classname = other.classname; //mxd
			this.isknown = true;
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
				this.args[i] = other.args[i];

			// Copy properties
			this.sprite = other.sprite;
			this.spriteframe = new SpriteFrameInfo[other.spriteframe.Length]; //mxd
			other.spriteframe.CopyTo(this.spriteframe, 0); //mxd
			this.color = other.color;
			this.alpha = other.alpha; //mxd
			this.alphabyte = other.alphabyte; //mxd
			this.renderstyle = other.renderstyle; //mxd
			this.bright = other.bright; //mxd
			this.arrow = other.arrow;
			this.radius = other.radius;
			this.height = other.height;
			this.hangs = other.hangs;
			this.blocking = other.blocking;
			this.errorcheck = other.errorcheck;
			this.fixedsize = other.fixedsize;
			this.fixedrotation = other.fixedrotation; //mxd
			this.absolutez = other.absolutez;
			this.xybillboard = other.xybillboard; //mxd
			this.spritescale = new SizeF(other.spritescale.Width, other.spritescale.Height);

			//mxd. Copy GLOOME properties
			this.rendermode = other.rendermode;
			this.sticktoplane = other.sticktoplane;
			this.rollsprite = other.rollsprite;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This updates the properties from a decorate actor
		internal void ModifyByDecorateActor(ActorStructure actor)
		{
			// Keep reference to actor
			this.actor = actor;
			this.classname = actor.ClassName; //mxd
			
			// Set the title
			if(actor.HasPropertyWithValue("$title"))
				title = actor.GetPropertyAllValues("$title");
			else if(actor.HasPropertyWithValue("tag")) 
			{
				string tag = actor.GetPropertyAllValues("tag");
				if(!tag.StartsWith("\"$")) title = tag; //mxd. Don't use LANGUAGE keywords.
			}

			if(string.IsNullOrEmpty(title)) title = actor.ClassName;
				
			//mxd. Color override?
			if(actor.HasPropertyWithValue("$color")) 
			{
				int ci = actor.GetPropertyValueInt("$color", 0);
				color = (ci == 0 || ci > 19 ? 18 : ci) ;
			}

			//mxd. Custom argument titles?
			for(int i = 0; i < args.Length; i++)
			{
				if(!actor.HasPropertyWithValue("$arg" + i)) continue;
				string argtitle = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i));
				string argtooltip = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "tooltip").Replace("\\n", Environment.NewLine));
				int argtype = actor.GetPropertyValueInt("$arg" + i + "type", 0);
				int defaultvalue = actor.GetPropertyValueInt("$arg" + i + "default", 0);
				string argenum = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "enum"));
				args[i] = new ArgumentInfo(title, argtitle, argtooltip, argtype, defaultvalue, argenum, General.Map.Config.Enums);
			}

			//mxd. Some SLADE compatibility
			if(actor.HasProperty("$angled")) this.arrow = true;
			else if(actor.HasProperty("$notangled")) this.arrow = false;

			//mxd. Marked as obsolete?
			if(actor.HasPropertyWithValue("$obsolete"))
			{
				obsoletemessage = actor.GetPropertyValueString("$obsolete", 0, true);
				obsolete = true;
				color = 4; //red
			}

			// Remove doublequotes from title
			title = ZDTextParser.StripQuotes(title); //mxd
			
			// Set sprite
			string suitablesprite = (locksprite ? string.Empty : actor.FindSuitableSprite()); //mxd
			if(!string.IsNullOrEmpty(suitablesprite)) 
				sprite = suitablesprite;
			else if(string.IsNullOrEmpty(sprite))//mxd
				sprite = DataManager.INTERNAL_PREFIX + "unknownthing";

			//mxd. Create sprite frame
			this.spriteframe = new[] { new SpriteFrameInfo { Sprite = sprite, SpriteLongName = Lump.MakeLongName(sprite, true) } };
			
			// Set sprite scale (mxd. Scale is translated to xscale and yscale in ActorStructure)
			if(actor.HasPropertyWithValue("xscale"))
				this.spritescale.Width = actor.GetPropertyValueFloat("xscale", 0);
			
			if(actor.HasPropertyWithValue("yscale"))
				this.spritescale.Height = actor.GetPropertyValueFloat("yscale", 0);
			
			// Size
			if(actor.HasPropertyWithValue("radius")) radius = actor.GetPropertyValueInt("radius", 0);
			if(actor.HasPropertyWithValue("height")) height = actor.GetPropertyValueInt("height", 0);

			//mxd. Renderstyle
			if(actor.HasPropertyWithValue("renderstyle") && !actor.HasProperty("$ignorerenderstyle"))
				renderstyle = actor.GetPropertyValueString("renderstyle", 0, true).ToLower();

			//mxd. Alpha
			if(actor.HasPropertyWithValue("alpha"))
			{
				this.alpha = General.Clamp(actor.GetPropertyValueFloat("alpha", 0), 0f, 1f);
				this.alphabyte = (byte)(this.alpha * 255);
			}
			else if(actor.HasProperty("defaultalpha"))
			{
				this.alpha = (General.Map.Config.GameType == GameType.HERETIC ? 0.4f : 0.6f);
				this.alphabyte = (byte)(this.alpha * 255);
			}

			//mxd. BRIGHT
			this.bright = actor.GetFlagValue("bright", false);
			
			// Safety
			if(this.radius < 4f || this.fixedsize) this.radius = THING_FIXED_SIZE;
			if(this.spritescale.Width == 0.0f) this.spritescale.Width = 1.0f;
			if(this.spritescale.Height == 0.0f) this.spritescale.Height = 1.0f;
			
			// Options
			hangs = actor.GetFlagValue("spawnceiling", hangs);
			int blockvalue = (blocking > 0) ? blocking : 2;
			blocking = actor.GetFlagValue("solid", (blocking != 0)) ? blockvalue : 0;
			xybillboard = actor.GetFlagValue("forcexybillboard", false); //mxd

			//mxd. GLOOME rendering flags. ORDER: WALLSPRITE -> FLOORSPRITE || CEILSPRITE
			rollsprite = actor.GetFlagValue("rollsprite", false); 
			if(actor.GetFlagValue("wallsprite", false)) rendermode = Thing.SpriteRenderMode.WALL_SPRITE;
			else if(actor.GetFlagValue("floorsprite", false)) rendermode = Thing.SpriteRenderMode.FLOOR_SPRITE;
			else if(actor.GetFlagValue("ceilsprite", false)) rendermode = Thing.SpriteRenderMode.CEILING_SPRITE;
			if(rendermode == Thing.SpriteRenderMode.FLOOR_SPRITE || rendermode == Thing.SpriteRenderMode.CEILING_SPRITE)
				sticktoplane = actor.GetFlagValue("sticktoplane", false); // Works only for Floor/Ceil sprites

			//mxd
			if(blocking > THING_BLOCKING_NONE) errorcheck = THING_ERROR_INSIDE_STUCK;
		}

		//mxd. This tries to find all possible sprite rotations
		internal void SetupSpriteFrame()
		{
			// Empty or internal sprites don't have rotations
			if(string.IsNullOrEmpty(sprite) || sprite.StartsWith(DataManager.INTERNAL_PREFIX)) return;

			// Skip sprites with strange names
			if(sprite.Length != 6 && sprite.Length != 8)
			{
				General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ". Unsupported sprite name fromat: \"" + sprite + "\"");
				return;
			}

			// Get sprite angle
			string anglestr = sprite.Substring(5, 1);
			int sourceangle;
			if(!int.TryParse(anglestr, NumberStyles.Integer, CultureInfo.InvariantCulture, out sourceangle))
			{
				General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ". Unable to get sprite angle from sprite \"" + sprite + "\"");
				return;
			}

			if(sourceangle < 0 || sourceangle > 8)
			{
				General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ", sprite \"" + sprite + "\". Sprite angle must be in [0..8] range");
				return;
			}

			// No rotations?
			if(sourceangle == 0) return;
			
			// Gather rotations
			string[] frames = new string[8];
			bool[] mirror = new bool[8];
			int processedcount = 0;
			string sourcename = sprite.Substring(0, 4);
			IEnumerable<string> spritenames = General.Map.Data.GetSpriteNames(sourcename);

			// Process gathered sprites
			char sourceframe = sprite[4];
			foreach(string s in spritenames)
			{
				//Check first frame block
				char targetframe = s[4];
				if(targetframe == sourceframe)
				{
					// Check angle
					int targetangle;
					anglestr = s.Substring(5, 1);
					if(!int.TryParse(anglestr, NumberStyles.Integer, CultureInfo.InvariantCulture, out targetangle))
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ". Unable to get sprite angle from sprite \"" + s + "\"");
						return;
					}

					if(targetangle == 0)
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Warning: actor \"" + title + "\":" + index + ", sprite \"" + sourcename + "\", frame " + targetframe + " has both rotated and non-rotated versions");
						continue;
					}

					if(targetangle < 1 || targetangle > 8)
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ", sprite \"" + s + "\". Expected sprite angle in [1..8] range");
						return;
					}

					// Add to collection
					frames[targetangle - 1] = s;
					processedcount++;
				}

				// Check second frame block?
				if(s.Length == 6) continue;

				targetframe = s[6];
				if(targetframe == sourceframe)
				{
					// Check angle
					int targetangle;
					anglestr = s.Substring(7, 1);
					if(!int.TryParse(anglestr, NumberStyles.Integer, CultureInfo.InvariantCulture, out targetangle))
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ". Unable to get sprite angle from sprite \"" + s + "\"");
						return;
					}

					if(targetangle == 0)
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Warning: actor \"" + title + "\":" + index + ", sprite \"" + sourcename + "\", frame " + targetframe + " has both rotated and non-rotated versions");
						continue;
					}

					if(targetangle < 1 || targetangle > 8)
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ", sprite \"" + s + "\". Expected sprite angle in [1..8] range");
						return;
					}

					// Add to collections
					frames[targetangle - 1] = s;
					mirror[targetangle - 1] = true;
					processedcount++;
				}

				// Gathered all sprites?
				if(processedcount == 8) break;
			}

			// Check collected data
			if(processedcount != 8)
			{
				// Check which angles are missing
				List<string> missingangles = new List<string>();
				for(int i = 0; i < frames.Length; i++)
				{
					if(string.IsNullOrEmpty(frames[i]))
						missingangles.Add((i + 1).ToString());
				}

				// Assemble angles to display
				string ma = string.Join(", ", missingangles.ToArray());
				if(missingangles.Count > 2)
				{
					int pos = ma.LastIndexOf(",", StringComparison.Ordinal);
					if(pos != -1) ma = ma.Remove(pos, 1).Insert(pos, " and");
				}

				General.ErrorLogger.Add(ErrorType.Error, "Error in actor \"" + title + "\":" + index + ". Sprite rotations " + ma + " for sprite " + sourcename + ", frame " + sourceframe + " are missing");
				return;
			}

			// Create collection
			spriteframe = new SpriteFrameInfo[frames.Length];
			for(int i = 0; i < frames.Length; i++)
			{
				spriteframe[i] = new SpriteFrameInfo { Sprite = frames[i], SpriteLongName = Lump.MakeLongName(frames[i]), Mirror = mirror[i] };
			}
		}

		// This is used for sorting
		public int CompareTo(ThingTypeInfo other)
		{
			return string.Compare(this.title, other.title, true);
		}
		
		// String representation
		public override string ToString()
		{
			return title + " (" + index + ")";
		}
		
		#endregion
	}
}
