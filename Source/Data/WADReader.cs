
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal class WADReader : DataReader
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Source
		private WAD file;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public WADReader(DataLocation dl) : base(dl)
		{
			// Initialize
			file = new WAD(location.location, true);

			General.WriteLogLine("Opening WAD resource '" + location.location + "'");

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing WAD resource '" + location.location + "'");

				// Clean up
				file.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// This suspends use of this resource
		public override void Suspend()
		{
			file.Dispose();
			base.Suspend();
		}

		// This resumes use of this resource
		public override void Resume()
		{
			file = new WAD(location.location, true);
			base.Resume();
		}
		
		#endregion

		#region ================== Palette

		// This loads the PLAYPAL palette
		public override Playpal LoadPalette()
		{
			Lump lump;
			
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");
			
			// Look for a lump named PLAYPAL
			lump = file.FindLump("PLAYPAL");
			if(lump != null)
			{
				// Read the PLAYPAL from stream
				return new Playpal(lump.Stream);
			}
			else
			{
				// No palette
				return null;
			}
		}

		#endregion

		#region ================== Textures

		// This loads the textures
		public override ICollection<ImageData> LoadTextures(PatchNames pnames)
		{
			List<ImageData> images = new List<ImageData>();
			Lump lump;
			
			// Load two sets of textures, if available
			lump = file.FindLump("TEXTURE1");
			if(lump != null) LoadTextureSet(lump.Stream, ref images, pnames);
			lump = file.FindLump("TEXTURE2");
			if(lump != null) LoadTextureSet(lump.Stream, ref images, pnames);
			
			// Return result
			return images;
		}
		
		// This loads a set of textures
		private void LoadTextureSet(Stream texturedata, ref List<ImageData> images, PatchNames pnames)
		{
			BinaryReader reader = new BinaryReader(texturedata);
			int flags, width, height, patches, px, py, pi;
			uint numtextures;
			byte scalebytex, scalebytey;
			float scalex, scaley, defaultscale;
			byte[] namebytes;
			TextureImage image;
			bool strifedata;
			
			// Determine default scale
			defaultscale = General.Map.Configuration.ReadSetting("defaulttexturescale", 1.0f);
			
			// Get number of textures
			texturedata.Seek(0, SeekOrigin.Begin);
			numtextures = reader.ReadUInt32();
			
			// Skip offset bytes (we will read all textures sequentially)
			texturedata.Seek(4 * numtextures, SeekOrigin.Current);

			// Go for all textures defined in this lump
			for(uint i = 0; i < numtextures; i++)
			{
				// Read texture properties
				namebytes = reader.ReadBytes(8);
				flags = reader.ReadUInt16();
				scalebytex = reader.ReadByte();
				scalebytey = reader.ReadByte();
				width = reader.ReadInt16();
				height = reader.ReadInt16();
				patches = reader.ReadInt16();
				
				// Check for doom or strife data format
				if(patches == 0)
				{
					// Ignore 2 bytes and then read number of patches
					texturedata.Seek(2, SeekOrigin.Current);
					patches = reader.ReadInt16();
					strifedata = false;
				}
				else
				{
					// Texture data is in strife format
					strifedata = true;
				}

				// Determine actual scales
				if(scalebytex == 0) scalex = defaultscale; else scalex = 1f / ((float)scalebytex / 8f);
				if(scalebytey == 0) scaley = defaultscale; else scaley = 1f / ((float)scalebytey / 8f);
				
				// Validate data
				if((width > 0) && (height > 0) && (patches > 0) &&
				   (scalex != 0) || (scaley != 0))
				{
					// Make the image object
					image = new TextureImage(Lump.MakeNormalName(namebytes, WAD.ENCODING),
											 width, height, scalex, scaley);

					// Go for all patches in texture
					for(int p = 0; p < patches; p++)
					{
						// Read patch properties
						px = reader.ReadInt16();
						py = reader.ReadInt16();
						pi = reader.ReadUInt16();
						if(!strifedata) texturedata.Seek(4, SeekOrigin.Current);
						
						// Validate data
						if((pi >= 0) && (pi < pnames.Length) && (pnames[pi].Length > 0))
						{
							// Create patch on image
							image.AddPatch(new TexturePatch(pnames[pi], px, py));
						}
					}
					
					// Add image to collection
					images.Add(image);
				}
				else
				{
					// Skip patches data
					texturedata.Seek(6 * patches, SeekOrigin.Current);
					if(!strifedata) texturedata.Seek(4 * patches, SeekOrigin.Current);
				}
			}
		}

		// This returns the patch names from the PNAMES lump
		public override PatchNames LoadPatchNames()
		{
			Lump lump;

			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Look for a lump named PNAMES
			lump = file.FindLump("PNAMES");
			if(lump != null)
			{
				// Read the PNAMES from stream
				return new PatchNames(lump.Stream);
			}
			else
			{
				// No palette
				return null;
			}
		}

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname)
		{
			Lump lump;
			
			// Find the lump
			lump = file.FindLump(pname);
			if(lump != null) return lump.Stream; else return null;
		}
		
		#endregion
	}
}
