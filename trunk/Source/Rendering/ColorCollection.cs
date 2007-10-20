
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
using System.Reflection;
using System.Drawing;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class ColorCollection
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Colors
		private Dictionary<string, ColorSetting> colors;
		
		#endregion

		#region ================== Properties

		private ColorSetting Background { get { return colors["background"]; } }
		private ColorSetting Vertex { get { return colors["vertex"]; } }
		private ColorSetting Linedef { get { return colors["linedef"]; } }
		private ColorSetting Highlight { get { return colors["highlight"]; } }
		private ColorSetting Selection { get { return colors["selection"]; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor for settings from configuration
		public ColorCollection(Configuration cfg)
		{
			IDictionary cs;
			
			// Initialize
			colors = new Dictionary<string, ColorSetting>();

			// Read all colors from config
			cs = cfg.ReadSetting("colors", new Hashtable());
			foreach(DictionaryEntry c in cs)
			{
				// Add color
				if(c.Value is int)
					colors.Add(c.Key.ToString(), new ColorSetting(c.Key.ToString(), PixelColor.FromInt((int)c.Value)));
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Copy constructor
		public ColorCollection(ColorCollection collection)
		{
			// Initialize
			colors = new Dictionary<string, ColorSetting>();

			// Go for all elements in the original collection
			foreach(KeyValuePair<string, ColorSetting> c in collection.colors)
			{
				// Copy
				colors.Add(c.Key, new ColorSetting(c.Key, c.Value.Color));
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This applies colors to this collection
		public void Apply(ColorCollection collection)
		{
			// Go for all elements in the original collection
			foreach(KeyValuePair<string, ColorSetting> c in collection.colors)
			{
				// Add or update
				colors[c.Key] = new ColorSetting(c.Key, c.Value.Color);
			}
		}
		
		// This saves colors to configuration
		public void SaveColors(Configuration cfg)
		{
			// Go for all elements in the original collection
			foreach(KeyValuePair<string, ColorSetting> c in colors)
			{
				// Write to configuration
				cfg.WriteSetting("colors." + c.Key, c.Value.Color.ToInt());
			}
		}
		
		#endregion
	}
}
