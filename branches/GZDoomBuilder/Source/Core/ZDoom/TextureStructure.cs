
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

using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class TextureStructure
	{
		#region ================== Constants

		// Some odd thing in ZDoom
		private const string IGNORE_SPRITE = "TNT1A0";

		#endregion

		#region ================== Variables

		// Declaration
		private readonly string typename;
		private readonly string name;
		private readonly string virtualpath; //mxd
		private readonly int width;
		private readonly int height;
		
		// Properties
		private readonly float xscale;
		private readonly float yscale;
		private readonly int xoffset;
		private readonly int yoffset;
		private readonly bool worldpanning;
		
		// Patches
		private readonly List<PatchStructure> patches;
		
		#endregion

		#region ================== Properties

		public string TypeName { get { return typename; } }
		public string Name { get { return name; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public float XScale { get { return xscale; } }
		public float YScale { get { return yscale; } }
		public int XOffset { get { return xoffset; } }
		public int YOffset { get { return yoffset; } }
		public bool WorldPanning { get { return worldpanning; } }
		public ICollection<PatchStructure> Patches { get { return patches; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal TextureStructure(TexturesParser parser, string typename, string virtualpath)
		{
			// Initialize
			this.typename = typename;
			this.virtualpath = virtualpath;
			patches = new List<PatchStructure>(4);
			xscale = 0.0f;
			yscale = 0.0f;
			
			// There should be 3 tokens separated by 2 commas now:
			// Name, Width, Height

			// First token is the texture name
			parser.SkipWhitespace(true);
			name = parser.StripTokenQuotes(parser.ReadToken(false)); //mxd. Don't skip newline

			//mxd. It can also be "optional" keyword.
			if(name.ToLowerInvariant() == "optional")
			{
				parser.SkipWhitespace(true);
				name = parser.StripTokenQuotes(parser.ReadToken(false)); //mxd. Don't skip newline
			}

			if(string.IsNullOrEmpty(name))
			{
				parser.ReportError("Expected " + typename + " name");
				return;
			}

			// Now we should find a comma
			parser.SkipWhitespace(true);
			string tokenstr = parser.ReadToken();
			if(tokenstr != ",")
			{
				parser.ReportError("Expected a comma");
				return;
			}

			// Next is the texture width
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(string.IsNullOrEmpty(tokenstr) || !int.TryParse(tokenstr, NumberStyles.Integer, CultureInfo.InvariantCulture, out width))
			{
				parser.ReportError("Expected width in pixels");
				return;
			}

			// Now we should find a comma again
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != ",")
			{
				parser.ReportError("Expected a comma");
				return;
			}

			// Next is the texture height
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(string.IsNullOrEmpty(tokenstr) || !int.TryParse(tokenstr, NumberStyles.Integer, CultureInfo.InvariantCulture, out height))
			{
				parser.ReportError("Expected height in pixels");
				return;
			}

			// Next token should be the beginning of the texture scope
			parser.SkipWhitespace(true);
			tokenstr = parser.ReadToken();
			if(tokenstr != "{")
			{
				parser.ReportError("Expected begin of structure");
				return;
			}

			// Now parse the contents of texture structure
			bool done = false; //mxd
			while(!done && parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();

				switch(token) 
				{
					case "xscale":
						if(!ReadTokenFloat(parser, token, out xscale)) return;
						break;

					case "yscale":
						if(!ReadTokenFloat(parser, token, out yscale)) return;
						break;

					case "worldpanning":
						worldpanning = true;
						break;

					case "offset":
						// Read x offset
						if(!ReadTokenInt(parser, token, out xoffset)) return;

						// Now we should find a comma
						parser.SkipWhitespace(true);
						tokenstr = parser.ReadToken();
						if(tokenstr != ",") 
						{
							parser.ReportError("Expected a comma");
							return;
						}

						// Read y offset
						if(!ReadTokenInt(parser, token, out yoffset)) return;
						break;

					case "patch":
						// Read patch structure
						PatchStructure pt = new PatchStructure(parser);
						if(parser.HasError) break;

						//mxd. Let's ignore TNT1A0
						if(pt.Name == IGNORE_SPRITE) break;

						// Add the patch
						patches.Add(pt);
						break;

					case "}":
						// Actor scope ends here,
						// break out of this parse loop
						done = true;
						break;
				}
			}
		}

		#endregion

		#region ================== Methods

		// This reads the next token and sets a floating point value, returns false when failed
		private static bool ReadTokenFloat(TexturesParser parser, string propertyname, out float value)
		{
			// Next token is the property value to set
			parser.SkipWhitespace(true);
			string strvalue = parser.ReadToken();
			if(!string.IsNullOrEmpty(strvalue))
			{
				// Try parsing as value
				if(!float.TryParse(strvalue, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
				{
					parser.ReportError("Expected numeric value for property '" + propertyname + "'");
					return false;
				}
				else
				{
					// Success
					return true;
				}
			}
			else
			{
				// Can't find the property value!
				parser.ReportError("Expected a value for property '" + propertyname + "'");
				value = 0.0f;
				return false;
			}
		}

		// This reads the next token and sets an integral value, returns false when failed
		private static bool ReadTokenInt(TexturesParser parser, string propertyname, out int value)
		{
			// Next token is the property value to set
			parser.SkipWhitespace(true);
			string strvalue = parser.ReadToken();
			if(!string.IsNullOrEmpty(strvalue))
			{
				// Try parsing as value
				if(!int.TryParse(strvalue, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
				{
					parser.ReportError("Expected integral value for property '" + propertyname + "'");
					return false;
				}
				else
				{
					// Success
					return true;
				}
			}
			else
			{
				// Can't find the property value!
				parser.ReportError("Expected a value for property '" + propertyname + "'");
				value = 0;
				return false;
			}
		}

		// This makes a HighResImage texture for this texture
		internal HighResImage MakeImage()
		{
			float scalex, scaley;
			
			// Determine default scale
			float defaultscale = General.Map.Config.DefaultTextureScale;

			// Determine scale for texture
			if(xscale == 0.0f) scalex = defaultscale; else scalex = 1f / xscale;
			if(yscale == 0.0f) scaley = defaultscale; else scaley = 1f / yscale;

			// Make texture
			HighResImage tex = new HighResImage(name, virtualpath, width, height, scalex, scaley, worldpanning, typename == "flat");

			// Add patches
			foreach(PatchStructure p in patches)
			{
				tex.AddPatch(new TexturePatch(p));//mxd
			}
			
			return tex;
		}
		
		#endregion
	}
}
