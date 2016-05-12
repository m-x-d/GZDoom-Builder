
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
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	//mxd
	public class TextResourceData
	{
		private Stream stream;
		private DataReader source;
		private DataLocation sourcelocation;
		private string filename;
		private int lumpindex;
		private bool trackable;

		internal Stream Stream { get { return stream; } }
		internal DataReader Source { get { return source; } }
		internal DataLocation SourceLocation { get { return sourcelocation; } }
		internal string Filename { get { return filename; } } // Lump name/Filename
		internal int LumpIndex { get { return lumpindex; } } // Lump index in a WAD
		internal bool Trackable { get { return trackable; } set { trackable = value; } } // When false, wont be added to DataManager.TextResources


		internal TextResourceData(DataReader Source, Stream Stream, string Filename, bool Trackable)
		{
			source = Source;
			sourcelocation = Source.Location;
			stream = Stream;
			filename = Filename;
			trackable = Trackable;

			WADReader reader = source as WADReader;
			if(reader != null)
				lumpindex = reader.WadFile.FindLumpIndex(filename);
			else
				lumpindex = -1;
		}

		internal TextResourceData(DataReader Source, Stream Stream, string Filename, int LumpIndex, bool Trackable)
		{
			source = Source;
			sourcelocation = Source.Location;
			stream = Stream;
			filename = Filename;
			lumpindex = LumpIndex;
			trackable = Trackable;
		}

		internal TextResourceData(Stream Stream, DataLocation Location, string Filename, bool Trackable)
		{
			source = null;
			sourcelocation = Location;
			stream = Stream;
			filename = Filename;
			lumpindex = -1;
			trackable = Trackable;
		}
	}
	
	internal abstract class DataReader : IDisposable
	{
		#region ================== Constants

		protected const string SPRITE_NAME_PATTERN = "(?i)\\A[a-z0-9]{4}([a-z][0-9]{0,2})$"; //mxd

		#endregion

		#region ================== Variables

		protected DataLocation location;
		protected bool issuspended;
		protected bool isdisposed;
		protected bool isreadonly; //mxd
		protected ResourceTextureSet textureset;

		#endregion

		#region ================== Properties

		public DataLocation Location { get { return location; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }
		public bool IsReadOnly { get { return isreadonly; } } //mxd
		public ResourceTextureSet TextureSet { get { return textureset; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		protected DataReader(DataLocation dl, bool asreadonly)
		{
			// Keep information
			location = dl;
			isreadonly = asreadonly;
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
		public abstract PatchNames LoadPatchNames();

		// When implemented, this returns the patch lump
		public abstract Stream GetPatchData(string pname, bool longname, ref string patchlocation);

		// When implemented, this returns the texture lump
		public abstract Stream GetTextureData(string pname, bool longname, ref string texturelocation);

		// When implemented, this loads the textures
		public abstract IEnumerable<ImageData> LoadTextures(PatchNames pnames, Dictionary<string, TexturesParser> cachedparsers);

		//mxd. When implemented, this returns the HiRes texture lump
		public abstract Stream GetHiResTextureData(string pname, ref string hireslocation);

		//mxd. When implemented, this loads the HiRes textures
		public abstract IEnumerable<HiResImage> LoadHiResTextures();
		
		#endregion

		#region ================== Flats
		
		// When implemented, this loads the flats
		public abstract IEnumerable<ImageData> LoadFlats(Dictionary<string, TexturesParser> cachedparsers);

		// When implemented, this returns the flat lump
		public abstract Stream GetFlatData(string pname, bool longname, ref string flatlocation);
		
		#endregion
		
		#region ================== Sprites

		// When implemented, this loads the sprites
		public abstract IEnumerable<ImageData> LoadSprites(Dictionary<string, TexturesParser> cachedparsers);
		
		// When implemented, this returns the sprite lump
		public abstract Stream GetSpriteData(string pname, ref string spritelocation);

		// When implemented, this checks if the given sprite lump exists
		public abstract bool GetSpriteExists(string pname);

		//mxd. When implemented, returns all sprites, which name starts with given string
		public abstract HashSet<string> GetSpriteNames(string startswith);
		
		#endregion

		#region ================== Decorate, Modeldef, Mapinfo, Gldefs, etc...

		// When implemented, this returns the DECORATE lump
		public abstract IEnumerable<TextResourceData> GetDecorateData(string pname);

		//mxd. When implemented, this returns the MODELDEF lump
		public abstract IEnumerable<TextResourceData> GetModeldefData();

		//mxd. When implemented, this returns the MAPINFO lump
		public abstract IEnumerable<TextResourceData> GetMapinfoData();

		//mxd. When implemented, this returns the GLDEFS lump
		public abstract IEnumerable<TextResourceData> GetGldefsData(GameType gametype);

		//mxd. When implemented, this returns the REVERBS lump
		public abstract IEnumerable<TextResourceData> GetReverbsData();

		//mxd. When implemented, this returns the VOXELDEF lump
		public abstract IEnumerable<TextResourceData> GetVoxeldefData();

		//mxd. When implemented, this returns the SNDSEQ lump
		public abstract IEnumerable<TextResourceData> GetSndSeqData();

		//mxd. When implemented, this returns the ANIMDEFS lump
		public abstract IEnumerable<TextResourceData> GetAnimdefsData();

		//mxd. When implemented, this returns the TERRAIN lump
		public abstract IEnumerable<TextResourceData> GetTerrainData();

		//mxd. When implemented, this returns the X11R6RGB lump
		public abstract IEnumerable<TextResourceData> GetX11R6RGBData();

		//mxd. When implemented, this returns the CVARINFO lump
		public abstract IEnumerable<TextResourceData> GetCvarInfoData();

		//mxd. When implemented, this returns the list of voxel model names
		public abstract IEnumerable<string> GetVoxelNames();

		//mxd. When implemented, this returns the voxel lump
		public abstract Stream GetVoxelData(string name);

		#endregion

		#region ================== Load/Save (mxd)

		internal abstract MemoryStream LoadFile(string name);
		internal abstract MemoryStream LoadFile(string name, int lumpindex);
		internal abstract bool SaveFile(MemoryStream stream, string name);
		internal abstract bool SaveFile(MemoryStream stream, string name, int lumpindex);
		internal abstract bool FileExists(string filename);
		internal abstract bool FileExists(string filename, int lumpindex);

		#endregion

		#region ================== Compiling (mxd)

		internal abstract bool CompileLump(string lumpname, ScriptConfiguration scriptconfig, List<CompilerError> errors);
		internal abstract bool CompileLump(string lumpname, int lumpindex, ScriptConfiguration scriptconfig, List<CompilerError> errors);

		#endregion
	}
}
