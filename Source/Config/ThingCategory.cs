
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
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ThingCategory
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Things
		private List<ThingTypeInfo> things;
		
		// Category properties
		private string name;
		private string title;
		private bool sorted;

		// Thing properties for inheritance
		private string sprite;
		private int color;
		private int arrow;
		private float width;
		private float height;
		private int hangs;
		private int blocking;
		private int errorcheck;
		private bool fixedsize;

		// Disposing
		private bool isdisposed = false;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public string Sprite { get { return sprite; } }
		public bool Sorted { get { return sorted; } }
		public int Color { get { return color; } }
		public int Arrow { get { return arrow; } }
		public float Width { get { return width; } }
		public float Height { get { return height; } }
		public int Hangs { get { return hangs; } }
		public int Blocking { get { return blocking; } }
		public int ErrorCheck { get { return errorcheck; } }
		public bool FixedSize { get { return fixedsize; } }
		public bool IsDisposed { get { return isdisposed; } }
		public List<ThingTypeInfo> Things { get { return things; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ThingCategory(Configuration cfg, string name)
		{
			IDictionary dic;
			int index;
			
			// Initialize
			this.name = name;
			this.things = new List<ThingTypeInfo>();
			
			// Read properties
			this.title = cfg.ReadSetting("thingtypes." + name + ".title", "<category>");
			this.sprite = cfg.ReadSetting("thingtypes." + name + ".sprite", "");
			this.sorted = (cfg.ReadSetting("thingtypes." + name + ".sort", 0) != 0);
			this.color = cfg.ReadSetting("thingtypes." + name + ".color", 0);
			this.arrow = cfg.ReadSetting("thingtypes." + name + ".arrow", 0);
			this.width = cfg.ReadSetting("thingtypes." + name + ".width", 10);
			this.height = cfg.ReadSetting("thingtypes." + name + ".height", 20);
			this.hangs = cfg.ReadSetting("thingtypes." + name + ".hangs", 0);
			this.blocking = cfg.ReadSetting("thingtypes." + name + ".blocking", 0);
			this.errorcheck = cfg.ReadSetting("thingtypes." + name + ".error", 1);
			this.fixedsize = cfg.ReadSetting("thingtypes." + name + ".fixedsize", false);
			
			// Safety
			if(this.width < 8f) this.width = 8f;
			
			// Go for all items in category
			dic = cfg.ReadSetting("thingtypes." + name, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if the item key is numeric
				if(int.TryParse(de.Key.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out index))
				{
					// Check if the item value is a structure
					if(de.Value is IDictionary)
					{
						// Create this thing
						things.Add(new ThingTypeInfo(this, index, cfg));
					}
					// Check if the item value is a string
					else if(de.Value is string)
					{
						// Interpret this as the title
						things.Add(new ThingTypeInfo(this, index, de.Value.ToString()));
					}
				}
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				things = null;

				// Done
				isdisposed = true;
			}
		}
		
		#endregion

		#region ================== Methods

		// This adds a thing to the category
		internal void AddThing(ThingTypeInfo t)
		{
			// Add
			things.Add(t);
		}

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		#endregion
	}
}

