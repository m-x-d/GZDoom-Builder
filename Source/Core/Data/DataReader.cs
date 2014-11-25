
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
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal abstract class DataReader
	{
		#region ================== Constants

		protected const string SPRITE_NAME_PATTERN = "(?i)\\A[a-z0-9]{4}([a-z][0-9]{0,2})$"; //mxd

		#endregion

		#region ================== Variables

		protected DataLocation location;
		protected bool issuspended;
		protected bool isdisposed;
		protected ResourceTextureSet textureset;

		#endregion

		#region ================== Properties

		public DataLocation Location { get { return location; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }
		public ResourceTextureSet TextureSet { get { return textureset; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DataReader(DataLocation dl)
		{
			// Keep information
			location = dl;
			textureset = new ResourceTextureSet(GetTitle(), dl);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Done
				textureset = null;
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This returns a short name
		public abstract string GetTitle();

		// This suspends use of this resource
		public virtual void Suspend()
		{
			issuspended = true;
		}

		// This resumes use of this resource
		public virtual void Resume()
		{
			issuspended = false;
		}

		#endregion

		#region ================== Palette

		// When implemented, this should find and load a PLAYPAL palette
		public virtual Playpal LoadPalette() { return null; }
		
		#endregion

		#region ================== Colormaps

		// When implemented, this loads the colormaps
		public virtual ICollection<ImageData> LoadColormaps() { return null; }

		// When implemented, this returns the colormap lump
		public virtual Stream GetColormapData(string pname) { return null; }

		#endregion

		#region ================== Textures

		// When implemented, this should read the patch names
		public virtual PatchNames LoadPatchNames() { return null; }

		// When implemented, this returns the patch lump
		public virtual Stream GetPatchData(string pname, bool longname) { return null; }

		// When implemented, this returns the texture lump
		public virtual Stream GetTextureData(string pname, bool longname) { return null; }

		// When implemented, this loads the textures
		public virtual ICollection<ImageData> LoadTextures(PatchNames pnames) { return null; }
		
		#endregion

		#region ================== Flats
		
		// When implemented, this loads the flats
		public virtual ICollection<ImageData> LoadFlats() { return null; }

		// When implemented, this returns the flat lump
		public virtual Stream GetFlatData(string pname, bool longname) { return null; }
		
		#endregion
		
		#region ================== Sprites

		// When implemented, this loads the sprites
		public virtual ICollection<ImageData> LoadSprites() { return null; }
		
		// When implemented, this returns the sprite lump
		public virtual Stream GetSpriteData(string pname) { return null; }

		// When implemented, this checks if the given sprite lump exists
		public virtual bool GetSpriteExists(string pname) { return false; }
		
		#endregion

		#region ================== Decorate, Modeldef, Mapinfo, Gldefs, etc...

		// When implemented, this returns the decorate lump
		public virtual Dictionary<string, Stream> GetDecorateData(string pname) { return new Dictionary<string, Stream>(); }

		//mxd. When implemented, this returns the Modeldef lump
		public virtual Dictionary<string, Stream> GetModeldefData() { return new Dictionary<string, Stream>(); }

		//mxd. When implemented, this returns the Mapinfo lump
		public virtual Dictionary<string, Stream> GetMapinfoData() { return new Dictionary<string, Stream>(); }

		//mxd. When implemented, this returns the Gldefs lump
		public virtual Dictionary<string, Stream> GetGldefsData(GameType gameType) { return new Dictionary<string, Stream>(); }
		public virtual Dictionary<string, Stream> GetGldefsData(string location) { return new Dictionary<string, Stream>(); }

		//mxd. When implemented, this returns the list of voxel model names
		public virtual string[] GetVoxelNames() { return null; }

		//mxd. When implemented, this returns the voxel lump
		public virtual Stream GetVoxelData(string name) { return null; }

		//mxd
		public virtual KeyValuePair<string, Stream> GetVoxeldefData() { return new KeyValuePair<string,Stream>(); }

		//mxd
		internal virtual MemoryStream LoadFile(string name) { return null; }
		internal virtual bool FileExists(string filename) { return false; }

		#endregion
	}
}
