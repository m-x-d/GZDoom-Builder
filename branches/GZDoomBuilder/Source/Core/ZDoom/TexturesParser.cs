
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
using System.IO;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class TexturesParser : ZDTextParser
	{
		#region ================== Delegates

		#endregion
		
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		private readonly Dictionary<string, TextureStructure> textures;
		private readonly Dictionary<string, TextureStructure> flats;
		private readonly Dictionary<string, TextureStructure> sprites;
		private readonly char[] pathtrimchars = new[] {'_', '.', ' ', '-'}; //mxd

		#endregion
		
		#region ================== Properties

		public IEnumerable<TextureStructure> Textures { get { return textures.Values; } }
		public IEnumerable<TextureStructure> Flats { get { return flats.Values; } }
		public IEnumerable<TextureStructure> Sprites { get { return sprites.Values; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public TexturesParser()
		{
			// Syntax
			whitespace = "\n \t\r\u00A0"; //mxd. non-breaking space is also space :)
			specialtokens = ",{}\n";

			// Initialize
			textures = new Dictionary<string, TextureStructure>(StringComparer.Ordinal);
			flats = new Dictionary<string, TextureStructure>(StringComparer.Ordinal);
			sprites = new Dictionary<string, TextureStructure>(StringComparer.Ordinal);
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given stream
		// Returns false on errors
		public override bool Parse(Stream stream, string sourcefilename)
		{
			base.Parse(stream, sourcefilename);

			//mxd. Make vitrual path from filename
			string virtualpath = sourcefilename.Substring(8).TrimStart(pathtrimchars);
			if(virtualpath.ToLowerInvariant() == "txt") virtualpath = string.Empty;
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(!string.IsNullOrEmpty(objdeclaration))
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					switch(objdeclaration)
					{
						case "texture":
						{
							// Read texture structure
							TextureStructure tx = new TextureStructure(this, "texture", virtualpath);
							if(this.HasError) return false;

							// if a limit for the texture name length is set make sure that it's not exceeded
							if(tx.Name.Length > General.Map.Config.MaxTextureNameLength)
							{
								ReportError("Texture name \"" + tx.Name + "\" too long. Texture names must have a length of " + General.Map.Config.MaxTextureNameLength + " characters or less");
								return false;
							}

							//mxd. Can't load image without name
							if(string.IsNullOrEmpty(tx.Name))
							{
								ReportError("Can't load an unnamed texture. Please consider giving names to your resources.");
								return false;
							}

							// Add the texture
							textures[tx.Name] = tx;
							if(!General.Map.Config.MixTexturesFlats) flats[tx.Name] = tx; //mxd. If MixTexturesFlats is set, textures and flats will be mixed in DataManager anyway
						}
						break;

						case "sprite":
						{
							// Read sprite structure
							TextureStructure tx = new TextureStructure(this, "sprite", virtualpath);
							if(this.HasError) return false;

							// if a limit for the sprite name length is set make sure that it's not exceeded
							if(tx.Name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
							{
								ReportError("Sprite name \"" + tx.Name + "\" too long. Sprite names must have a length of " + DataManager.CLASIC_IMAGE_NAME_LENGTH + " characters or less");
								return false;
							}

							//mxd. Can't load image without name
							if(string.IsNullOrEmpty(tx.Name))
							{
								ReportError("Can't load an unnamed sprite. Please consider giving names to your resources.");
								return false;
							}

							// Add the sprite
							sprites[tx.Name] = tx;
						}
						break;

						case "walltexture":
						{
							// Read walltexture structure
							TextureStructure tx = new TextureStructure(this, "walltexture", virtualpath);
							if(this.HasError) return false;

							// if a limit for the walltexture name length is set make sure that it's not exceeded
							if(tx.Name.Length > General.Map.Config.MaxTextureNameLength)
							{
								ReportError("WallTexture name \"" + tx.Name + "\" too long. WallTexture names must have a length of " + General.Map.Config.MaxTextureNameLength + " characters or less");
								return false;
							}

							//mxd. Can't load image without name
							if(string.IsNullOrEmpty(tx.Name))
							{
								ReportError("Can't load an unnamed WallTexture. Please consider giving names to your resources.");
								return false;
							}

							// Add the walltexture
							if(!textures.ContainsKey(tx.Name) || (textures[tx.Name].TypeName != "texture"))
								textures[tx.Name] = tx;
						}
						break;

						case "flat":
						{
							// Read flat structure
							TextureStructure tx = new TextureStructure(this, "flat", virtualpath);
							if(this.HasError) return false;

							// if a limit for the flat name length is set make sure that it's not exceeded
							if(tx.Name.Length > General.Map.Config.MaxTextureNameLength)
							{
								ReportError("Flat name \"" + tx.Name + "\" too long. Flat names must have a length of " + General.Map.Config.MaxTextureNameLength + " characters or less");
								return false;
							}

							//mxd. Can't load image without name
							if(string.IsNullOrEmpty(tx.Name))
							{
								ReportError("Can't load an unnamed flat. Please consider giving names to your resources.");
								return false;
							}

							// Add the flat
							if(!flats.ContainsKey(tx.Name) || (flats[tx.Name].TypeName != "texture"))
								flats[tx.Name] = tx;
						}
						break;

						case "$gzdb_skip": break;
						
						default:
						{
							// Unknown structure!
							// Best we can do now is just find the first { and then
							// follow the scopes until the matching } is found
							string token2;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
							}
							while(token2 != "{");

							int scopelevel = 1;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
								if(token2 == "{") scopelevel++;
								if(token2 == "}") scopelevel--;
							}
							while(scopelevel > 0);
						}
						break;
					}
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}

		//mxd
		protected override string GetLanguageType()
		{
			return "TEXTURES";
		}
		
		#endregion
	}
}
