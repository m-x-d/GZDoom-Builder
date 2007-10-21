
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
using SlimDX.Direct3D;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class ColorCollection
	{
		#region ================== Constants

		private const float BRIGHT_MULTIPLIER = 1.0f;
		private const float BRIGHT_ADDITION = 0.4f;
		private const float DARK_MULTIPLIER = 0.9f;
		private const float DARK_ADDITION = -0.2f;

		#endregion

		#region ================== Variables

		// Colors
		private Dictionary<string, ColorSetting> colors;
		private Dictionary<string, ColorSetting> brightcolors;
		private Dictionary<string, ColorSetting> darkcolors;
		
		#endregion

		#region ================== Properties

		public Dictionary<string, ColorSetting> ColorByName { get { return colors; } }
		public Dictionary<string, ColorSetting> BrightColorByName { get { return brightcolors; } }
		public Dictionary<string, ColorSetting> DarkColorByName { get { return darkcolors; } }
		
		public ColorSetting Background { get { return colors["background"]; } }
		public ColorSetting Vertices { get { return colors["vertices"]; } }
		public ColorSetting VerticesBright { get { return brightcolors["vertices"]; } }
		public ColorSetting VerticesDark { get { return darkcolors["vertices"]; } }
		public ColorSetting Linedefs { get { return colors["linedefs"]; } }
		public ColorSetting Actions { get { return colors["actions"]; } }
		public ColorSetting Sounds { get { return colors["sounds"]; } }
		public ColorSetting Highlight { get { return colors["highlight"]; } }
		public ColorSetting HighlightBright { get { return brightcolors["highlight"]; } }
		public ColorSetting HighlightDark { get { return darkcolors["highlight"]; } }
		public ColorSetting Selection { get { return colors["selection"]; } }
		public ColorSetting SelectionBright { get { return brightcolors["selection"]; } }
		public ColorSetting SelectionDark { get { return darkcolors["selection"]; } }
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
			brightcolors = new Dictionary<string, ColorSetting>();
			darkcolors = new Dictionary<string, ColorSetting>();
			
			// Read all colors from config
			cs = cfg.ReadSetting("colors", new Hashtable());
			foreach(DictionaryEntry c in cs)
			{
				// Add color
				if(c.Value is int)
					colors.Add(c.Key.ToString(), new ColorSetting(c.Key.ToString(), PixelColor.FromInt((int)c.Value)));
			}

			// Create assist colors
			CreateAssistColors();
			
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
		
		// This clamps a value between 0 and 1
		private float Saturate(float v)
		{
			if(v < 0f) return 0f; else if(v > 1f) return 1f; else return v;
		}

		// This creates assist colors
		public void CreateAssistColors()
		{
			ColorValue o;
			ColorValue c = new ColorValue(1f, 0f, 0f, 0f);
			
			// Go for all colors
			foreach(KeyValuePair<string, ColorSetting> cc in colors)
			{
				// Get original color
				o = ColorValue.FromColor(cc.Value.Color);

				// Create brighter color
				c.Red = Saturate(o.Red * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
				c.Green = Saturate(o.Green * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
				c.Blue = Saturate(o.Blue * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
				brightcolors[cc.Key] = new ColorSetting(cc.Key, PixelColor.FromInt(c.ToArgb()));

				// Create darker color
				c.Red = Saturate(o.Red * DARK_MULTIPLIER + DARK_ADDITION);
				c.Green = Saturate(o.Green * DARK_MULTIPLIER + DARK_ADDITION);
				c.Blue = Saturate(o.Blue * DARK_MULTIPLIER + DARK_ADDITION);
				darkcolors[cc.Key] = new ColorSetting(cc.Key, PixelColor.FromInt(c.ToArgb()));
			}
		}
		
		// This applies colors to this collection
		public void Apply(ColorCollection collection)
		{
			// Go for all elements in the original collection
			foreach(KeyValuePair<string, ColorSetting> c in collection.colors)
			{
				// Add or update
				colors[c.Key] = new ColorSetting(c.Key, c.Value.PixelColor);
			}

			// Rebuild assist colors
			CreateAssistColors();
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
