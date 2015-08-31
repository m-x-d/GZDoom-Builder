
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
using System.Globalization;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.Map;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ThingTypeInfo : INumberedTitle, IComparable<ThingTypeInfo>
	{
		#region ================== Constants

		public const int THING_BLOCKING_NONE = 0;
		public const int THING_BLOCKING_FULL = 1;
		public const int THING_BLOCKING_HEIGHT = 2;
		public const int THING_ERROR_NONE = 0;
		public const int THING_ERROR_INSIDE = 1;
		public const int THING_ERROR_INSIDE_STUCK = 2;
		
		#endregion

		#region ================== Variables

		// Properties
		private readonly int index;
		private string title;
		private string sprite;
		private ActorStructure actor;
		private string classname; //mxd
		private long spritelongname;
		private int color;
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

		//mxd. GLOOME rendering settings
		private Thing.SpriteRenderMode rendermode;
		private bool rollsprite;
		private bool sticktoplane;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Title { get { return title; } }
		public string Sprite { get { return sprite; } }
		public ActorStructure Actor { get { return actor; } }
		public long SpriteLongName { get { return spritelongname; } }
		public int Color { get { return color; } }
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
			this.arrow = true;
			this.radius = 10f;
			this.height = 20f;
			this.hangs = false;
			this.blocking = 0;
			this.errorcheck = 0;
			this.spritescale = new SizeF(1.0f, 1.0f);
			this.fixedsize = false;
			this.fixedrotation = false; //mxd
			this.spritelongname = long.MaxValue;
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
		
			// Read properties
			this.title = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".title", "<" + key + ">");
			this.sprite = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".sprite", cat.Sprite);
			this.color = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".color", cat.Color);
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
			if(this.radius < 4f) this.radius = 16f;
			if(this.hangs && this.absolutez) this.hangs = false; //mxd
			
			// Make long name for sprite lookup
			if(this.sprite.Length <= 8)
				this.spritelongname = Lump.MakeLongName(this.sprite);
			else
				this.spritelongname = long.MaxValue;
			
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
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++) this.args[i] = new ArgumentInfo(i);
			
			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
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
			this.locksprite = false;

			// Safety
			if(this.radius < 4f) this.radius = 8f;
			if(this.hangs && this.absolutez) this.hangs = false; //mxd
			
			// Make long name for sprite lookup
			if(this.sprite.Length <= 8)
				this.spritelongname = Lump.MakeLongName(this.sprite);
			else
				this.spritelongname = long.MaxValue;

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
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++) this.args[i] = new ArgumentInfo(i);
			
			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
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
			if(this.radius < 4f) this.radius = 8f;
			if(this.hangs && this.absolutez) this.hangs = false; //mxd
			
			// Apply settings from actor
			ModifyByDecorateActor(actor);
			
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
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++) this.args[i] = new ArgumentInfo(i);

			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
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
			if(this.radius < 4f) this.radius = 8f;
			if(this.hangs && this.absolutez) this.hangs = false; //mxd

			// Apply settings from actor
			ModifyByDecorateActor(actor);

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
			this.color = other.color;
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
			else if (actor.HasPropertyWithValue("tag")) 
			{
				string tag = actor.GetPropertyAllValues("tag");
				if(!tag.StartsWith("\"$")) title = tag; //mxd. Don't use LANGUAGE keywords.
			}

			if(string.IsNullOrEmpty(title)) title = actor.ClassName;
				
			//mxd. Color override?
			if (actor.HasPropertyWithValue("$color")) 
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

			// Remove doublequotes from title
			title = ZDTextParser.StripQuotes(title); //mxd
			
			// Set sprite
			string suitablesprite = (locksprite ? string.Empty : actor.FindSuitableSprite()); //mxd
			if(!string.IsNullOrEmpty(suitablesprite)) 
				sprite = suitablesprite;
			else if(string.IsNullOrEmpty(sprite))//mxd
				sprite = DataManager.INTERNAL_PREFIX + "unknownthing";

			
			if(this.sprite.Length < 9)
				this.spritelongname = Lump.MakeLongName(this.sprite);
			else
				this.spritelongname = long.MaxValue;
			
			// Set sprite scale (mxd. Scale is translated to xscale and yscale in ActorStructure)
			if(actor.HasPropertyWithValue("xscale"))
				this.spritescale.Width = actor.GetPropertyValueFloat("xscale", 0);
			
			if(actor.HasPropertyWithValue("yscale"))
				this.spritescale.Height = actor.GetPropertyValueFloat("yscale", 0);
			
			// Size
			if(actor.HasPropertyWithValue("radius")) radius = actor.GetPropertyValueInt("radius", 0);
			if(actor.HasPropertyWithValue("height")) height = actor.GetPropertyValueInt("height", 0);
			
			// Safety
			if(this.radius < 4f) this.radius = 8f;
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
