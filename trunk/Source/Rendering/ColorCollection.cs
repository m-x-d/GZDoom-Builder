
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

		public ColorSetting Background { get { return colors["background"]; } }
		public ColorSetting Vertices { get { return colors["vertices"]; } }
		public ColorSetting Linedefs { get { return colors["linedefs"]; } }
		public ColorSetting Actions { get { return colors["actions"]; } }
		public ColorSetting Sounds { get { return colors["sounds"]; } }
		public ColorSetting Highlight { get { return colors["highlight"]; } }
		public ColorSetting Selection { get { return colors["selection"]; } }
		public ColorSetting Association { get { return colors["association"]; } }
		public ColorSetting Grid { get { return colors["grid"]; } }
		public ColorSetting Grid64 { get { return colors["grid64"]; } }
		
		public ColorSetting Crosshair3D { get { return colors["crosshair3d"]; } }
		public ColorSetting Highlight3D { get { return colors["highlight3d"]; } }
		public ColorSetting Selection3D { get { return colors["selection3d"]; } }

		public ColorSetting ScriptBackground { get { return colors["scriptbackground"]; } }
		public ColorSetting LineNumbers { get { return colors["linenumbers"]; } }
		public ColorSetting PlainText { get { return colors["plaintext"]; } }
		public ColorSetting Comments { get { return colors["coments"]; } }
		public ColorSetting Keywords { get { return colors["keywords"]; } }
		public ColorSetting Literals { get { return colors["literals"]; } }
		public ColorSetting Constants { get { return colors["constants"]; } }

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
				colors.Add(c.Key, new ColorSetting(c.Key, c.Value.PixelColor));
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
				colors[c.Key] = new ColorSetting(c.Key, c.Value.PixelColor);
			}
		}
		
		// This saves colors to configuration
		public void SaveColors(Configuration cfg)
		{
			// Go for all elements in the original collection
			foreach(KeyValuePair<string, ColorSetting> c in colors)
			{
				// Write to configuration
				cfg.WriteSetting("colors." + c.Key, c.Value.PixelColor.ToInt());
			}
		}
		
		#endregion
	}
}
