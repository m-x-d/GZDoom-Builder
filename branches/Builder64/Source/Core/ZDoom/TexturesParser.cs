
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
using CodeImp.DoomBuilder.Compilers;

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

		private List<TextureStructure> textures;
		private List<TextureStructure> sprites;

		#endregion
		
		#region ================== Properties

		public ICollection<TextureStructure> Textures { get { return textures; } }
		public ICollection<TextureStructure> Sprites { get { return sprites; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public TexturesParser()
		{
			// Syntax
			whitespace = "\n \t\r";
			specialtokens = ",{}\n";

			// Initialize
			textures = new List<TextureStructure>();
			sprites = new List<TextureStructure>();
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given stream
		// Returns false on errors
		public override bool Parse(Stream stream, string sourcefilename)
		{
			base.Parse(stream, sourcefilename);
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(objdeclaration != null)
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					if(objdeclaration == "texture")
					{
						// Read texture structure
						TextureStructure tx = new TextureStructure(this);
						if(this.HasError) break;

						// if a limit for the texture name length is set make sure that it's not exceeded
						if ((General.Map.Config.MaxTextureNamelength > 0) && (tx.Name.Length > General.Map.Config.MaxTextureNamelength))
						{
							General.ErrorLogger.Add(ErrorType.Error, "Texture name \"" + tx.Name + "\" too long. Texture names must have a length of " + General.Map.Config.MaxTextureNamelength.ToString() + " characters or less");
						}
						else
						{
							// Add the texture
							textures.Add(tx);
						}
					}
					else if(objdeclaration == "sprite")
					{
						// Read sprite structure
						TextureStructure tx = new TextureStructure(this);
						if(this.HasError) break;

						// if a limit for the sprite name length is set make sure that it's not exceeded
						if ((General.Map.Config.MaxTextureNamelength > 0) && (tx.Name.Length > General.Map.Config.MaxTextureNamelength))
						{
							General.ErrorLogger.Add(ErrorType.Error, "Sprite name \"" + tx.Name + "\" too long. Sprite names must have a length of " +  General.Map.Config.MaxTextureNamelength.ToString() + " characters or less");
						}
						else
						{
							// Add the sprite
							sprites.Add(tx);
						}
					}
					else
					{
						// Unknown structure!
						// Best we can do now is just find the first { and then
						// follow the scopes until the matching } is found
						string token2;
						do
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(token2 == null) break;
						}
						while(token2 != "{");
						int scopelevel = 1;
						do
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(token2 == null) break;
							if(token2 == "{") scopelevel++;
							if(token2 == "}") scopelevel--;
						}
						while(scopelevel > 0);
					}
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}
		
		#endregion
	}
}
