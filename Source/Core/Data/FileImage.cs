
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
using System.IO;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class FileImage : ImageData
	{
		#region ================== Variables

		private readonly int probableformat;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FileImage(string name, string filepathname, bool asflat)
		{
			// Initialize
			this.isFlat = asflat; //mxd
			SetName(name, filepathname);

			if(asflat)
			{
				probableformat = ImageDataFormat.DOOMFLAT;
				this.scale.x = General.Map.Config.DefaultFlatScale;
				this.scale.y = General.Map.Config.DefaultFlatScale;
			}
			else
			{
				probableformat = ImageDataFormat.DOOMPICTURE;
				this.scale.x = General.Map.Config.DefaultTextureScale;
				this.scale.y = General.Map.Config.DefaultTextureScale;
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public FileImage(string name, string filepathname, bool asflat, float scalex, float scaley)
		{
			// Initialize
			this.scale.x = scalex;
			this.scale.y = scaley;
			this.isFlat = asflat; //mxd
			SetName(name, filepathname);

			probableformat = (asflat ? ImageDataFormat.DOOMFLAT : ImageDataFormat.DOOMPICTURE);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		//mxd. Constructor for loading internal images
		internal FileImage(string name, string filepathname)
		{
			// Initialize
			SetName(name, filepathname, true, true);

			probableformat = ImageDataFormat.DOOMPICTURE;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		//mxd: name is relative path to the image ("\Textures\sometexture.png")
		//mxd: filepathname is absolute path to the image ("D:\Doom\MyCoolProject\Textures\sometexture.png")
		//mxd: also, zdoom uses '/' as directory separator char.
		//mxd: and doesn't recognize long texture names in a root folder / pk3/7 root
		private void SetName(string name, string filepathname)
		{
			SetName(name, filepathname, General.Map.Config.UseLongTextureNames, false);
		}

		private void SetName(string name, string filepathname, bool uselongtexturenames, bool forcelongtexturename)
		{
			name = name.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			if(!uselongtexturenames || (!forcelongtexturename && string.IsNullOrEmpty(Path.GetDirectoryName(name))))
			{
				this.name = Path.GetFileNameWithoutExtension(name.ToUpperInvariant());
				if (this.name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
				{
					this.name = this.name.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
				}
				this.virtualname = Path.Combine(Path.GetDirectoryName(name), this.name).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				this.displayname = this.name;
				this.shortname = this.name;
			}
			else
			{
				this.name = name;
				this.virtualname = name;
				this.displayname = Path.GetFileNameWithoutExtension(name);
				this.shortname = this.displayname.ToUpperInvariant();
				if (this.shortname.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
				{
					this.shortname = this.shortname.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
				}
				hasLongName = true;
			}

			this.longname = Lump.MakeLongName(this.name, uselongtexturenames);
			this.fullname = filepathname;
			this.level = virtualname.Split(new[] { Path.AltDirectorySeparatorChar }).Length - 1;

			if(General.Settings.CapitalizeTextureNames && !string.IsNullOrEmpty(this.displayname))
			{
				this.displayname = this.displayname.ToUpperInvariant();
			}

			if (this.displayname.Length > ImageBrowserItem.MAX_NAME_LENGTH)
			{
				this.displayname = this.displayname.Substring(0, ImageBrowserItem.MAX_NAME_LENGTH);
			}
		}

		// This loads the image
		protected override void LocalLoadImage()
		{
			// Leave when already loaded
			if(this.IsImageLoaded) return;

			lock(this)
			{
				// Load file data
				if(bitmap != null) bitmap.Dispose(); bitmap = null;

				MemoryStream filedata = new MemoryStream(File.ReadAllBytes(fullname));

				// Get a reader for the data
				IImageReader reader = ImageDataFormat.GetImageReader(filedata, probableformat, General.Map.Data.Palette);
				if(!(reader is UnknownImageReader)) 
				{
					// Load the image
					filedata.Seek(0, SeekOrigin.Begin);
					try { bitmap = reader.ReadAsBitmap(filedata); } 
					catch(InvalidDataException) 
					{
						// Data cannot be read!
						bitmap = null;
					}
				}

				// Not loaded?
				if(bitmap == null) 
				{
					General.ErrorLogger.Add(ErrorType.Error, "Image file '" + fullname + "' data format could not be read, while loading image '" + this.Name + "'. Is this a valid picture file at all?");
					loadfailed = true;
				} 
				else 
				{
					// Get width and height
					width = bitmap.Size.Width;
					height = bitmap.Size.Height;
				}

				// Pass on to base
				filedata.Dispose();
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
