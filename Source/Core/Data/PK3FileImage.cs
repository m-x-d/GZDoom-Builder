
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class PK3FileImage : ImageData
	{
		#region ================== Variables

		private readonly PK3Reader datareader;
		private readonly int probableformat;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal PK3FileImage(PK3Reader datareader, string filepathname, bool asflat)
		{
			// Initialize
			this.datareader = datareader;
			this.isFlat = asflat; //mxd
			SetName(filepathname);

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

		#endregion

		#region ================== Methods

		//mxd: filepathname is relative path to the image ("Textures\sometexture.png")
		protected override void SetName(string filepathname) 
		{
			if(!General.Map.Config.UseLongTextureNames) 
			{
				this.name = Path.GetFileNameWithoutExtension(filepathname.ToUpperInvariant());
				if (this.name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
				{
					this.name = this.name.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
				}
				this.displayname = this.name;
				this.shortname = this.name;
			} 
			else 
			{
				this.name = filepathname.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				this.displayname = Path.GetFileNameWithoutExtension(name);
				this.shortname = this.displayname.ToUpperInvariant();
				if (this.shortname.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
				{
					this.shortname = this.shortname.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
				}
				this.hasLongName = true;
			}

			this.longname = Lump.MakeLongName(this.name);
			this.virtualname = filepathname.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			this.fullname = filepathname;

			if(General.Settings.CapitalizeTextureNames && !string.IsNullOrEmpty(this.displayname)) 
			{
				this.displayname = this.displayname.ToUpperInvariant();
			}

			if(this.displayname.Length > ImageBrowserItem.MAX_NAME_LENGTH) 
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
				MemoryStream filedata = datareader.LoadFile(fullname); //mxd
				
				if(filedata != null)
				{
					// Get a reader for the data
					IImageReader reader = ImageDataFormat.GetImageReader(filedata, probableformat, General.Map.Data.Palette);
					if(!(reader is UnknownImageReader))
					{
						// Load the image
						filedata.Seek(0, SeekOrigin.Begin);
						try
						{
							bitmap = reader.ReadAsBitmap(filedata);
						}
						catch (InvalidDataException)
						{
							// Data cannot be read!
							bitmap = null;
						}
					}

					// Not loaded?
					if(bitmap == null)
					{
						General.ErrorLogger.Add(ErrorType.Error, "Image file '" + fullname + "' data format could not be read, while loading texture '" + this.Name + "'");
						loadfailed = true;
					}
					else
					{
						// Get width and height from image
						width = bitmap.Size.Width;
						height = bitmap.Size.Height;
					}

					filedata.Dispose();
				}

				// Pass on to base
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
