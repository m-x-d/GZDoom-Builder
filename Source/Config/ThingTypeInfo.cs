
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
using CodeImp.DoomBuilder.Decorate;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ThingTypeInfo
	{
		#region ================== Constants

		public const int THING_BLOCKING_NONE = 0;
		public const int THING_BLOCKING_FULL = 1;
		public const int THING_BLOCKING_HEIGHT = 2;
		public const int THING_ERROR_NONE = 0;
		public const int THING_ERROR_INSIDE = 1;
		public const int THING_ERROR_INSIDE_STUCKED = 2;
		
		#endregion

		#region ================== Variables

		// Properties
		private int index;
		private string title;
		private string sprite;
		private long spritelongname;
		private int color;
		private bool arrow;
		private float width;
		private float height;
		private bool hangs;
		private int blocking;
		private int errorcheck;
		private bool fixedsize;
		private ThingCategory category;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Title { get { return title; } }
		public string Sprite { get { return sprite; } }
		public long SpriteLongName { get { return spritelongname; } }
		public int Color { get { return color; } }
		public bool Arrow { get { return arrow; } }
		public float Width { get { return width; } }
		public float Height { get { return height; } }
		public bool Hangs { get { return hangs; } }
		public int Blocking { get { return blocking; } }
		public int ErrorCheck { get { return errorcheck; } }
		public bool FixedSize { get { return fixedsize; } }
		public ThingCategory Category { get { return category; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ThingTypeInfo(int index)
		{
			// Initialize
			this.index = index;
			this.category = null;
			this.title = "<" + index.ToString(CultureInfo.InvariantCulture) + ">";
			this.sprite = DataManager.INTERNAL_PREFIX + "unknownthing";
			this.color = 0;
			this.arrow = true;
			this.width = 10f;
			this.height = 20f;
			this.hangs = false;
			this.blocking = 0;
			this.errorcheck = 0;
			this.fixedsize = false;
			this.spritelongname = long.MaxValue;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		internal ThingTypeInfo(ThingCategory cat, int index, Configuration cfg)
		{
			string key = index.ToString(CultureInfo.InvariantCulture);
			
			// Initialize
			this.index = index;
			this.category = cat;

			// Read properties
			this.title = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".title", "<" + key + ">");
			this.sprite = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".sprite", cat.Sprite);
			this.color = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".color", cat.Color);
			this.arrow = (cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".arrow", cat.Arrow) != 0);
			this.width = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".width", cat.Width);
			this.height = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".height", cat.Height);
			this.hangs = (cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".hangs", cat.Hangs) != 0);
			this.blocking = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".blocking", cat.Blocking);
			this.errorcheck = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".error", cat.ErrorCheck);
			this.fixedsize = cfg.ReadSetting("thingtypes." + cat.Name + "." + key + ".fixedsize", cat.FixedSize);
			
			// Safety
			if(this.width < 8f) this.width = 8f;
			
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
			string key = index.ToString(CultureInfo.InvariantCulture);

			// Initialize
			this.index = index;
			this.category = cat;
			this.title = title;

			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
			this.arrow = (cat.Arrow != 0);
			this.width = cat.Width;
			this.height = cat.Height;
			this.hangs = (cat.Hangs != 0);
			this.blocking = cat.Blocking;
			this.errorcheck = cat.ErrorCheck;
			this.fixedsize = cat.FixedSize;

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
			this.title = "Unnamed";
			
			// Read properties
			this.sprite = cat.Sprite;
			this.color = cat.Color;
			this.arrow = (cat.Arrow != 0);
			this.width = cat.Width;
			this.height = cat.Height;
			this.hangs = (cat.Hangs != 0);
			this.blocking = cat.Blocking;
			this.errorcheck = cat.ErrorCheck;
			this.fixedsize = cat.FixedSize;
			
			// Apply settings from actor
			ModifyByDecorateActor(actor);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This updates the properties from a decorate actor
		internal void ModifyByDecorateActor(ActorStructure actor)
		{
			// Set the title
			if(actor.Tag != null)
				title = actor.Tag;
			else
				title = actor.ClassName;
			
			// Set sprite
			sprite = actor.FindSuitableSprite();
			
			if(this.sprite.Length <= 8)
				this.spritelongname = Lump.MakeLongName(this.sprite);
			else
				this.spritelongname = long.MaxValue;
			
			// Size
			width = actor.Radius;
			height = actor.Height;
			
			// Safety
			if(this.width < 8f) this.width = 8f;
			
			// Options
			hangs = actor.GetFlagValue("spawnceiling", false);
			blocking = actor.GetFlagValue("solid", false) ? 2 : 0;
		}
		
		#endregion
	}
}
