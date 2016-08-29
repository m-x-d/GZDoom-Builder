﻿#region ================== Namespaces

using System;
using System.Drawing;
using System.IO;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class HiResImage : ImageData
	{
		#region ================== Variables

		private Vector2D sourcescale;
		private Size sourcesize;
		private bool overridesettingsapplied;

		#endregion

		#region ================== Properties

		public override int Width { get { return sourcesize.Width; } }
		public override int Height { get { return sourcesize.Height; } }
		public override float ScaledWidth { get { return (float)Math.Round(sourcesize.Width * sourcescale.x); } }
		public override float ScaledHeight { get { return (float)Math.Round(sourcesize.Height * sourcescale.y); } }
		public override Vector2D Scale { get { return sourcescale; } }

		#endregion

		#region ================== Constructor / Disposer

		public HiResImage(string name)
		{
			// Initialize
			this.scale.x = General.Map.Config.DefaultTextureScale;
			this.scale.y = General.Map.Config.DefaultTextureScale;
			this.sourcescale = scale;
			this.sourcesize = Size.Empty;
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Copy constructor
		public HiResImage(HiResImage other)
		{
			// Initialize
			this.scale = other.scale;
			this.sourcescale = other.sourcescale;
			this.sourcesize = other.sourcesize;

			// Copy names
			this.name = other.name;
			this.filepathname = other.filepathname;
			this.virtualname = other.virtualname;
			this.displayname = other.displayname;
			this.shortname = other.shortname;
			this.longname = other.longname;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		protected override void SetName(string name)
		{
			name = name.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			this.filepathname = name;
			this.name = Path.GetFileNameWithoutExtension(name.ToUpperInvariant());
			if(this.name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
			{
				this.name = this.name.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
			}
			this.virtualname = this.name;
			this.displayname = this.name;
			this.shortname = this.name;
			this.longname = Lump.MakeLongName(this.name);
		}

		internal void ApplySettings(ImageData overridden)
		{
			// Copy relevant names...
			name = overridden.Name;
			virtualname = overridden.VirtualName;
			displayname = overridden.DisplayName;

			isFlat = overridden.IsFlat;
			hasLongName = overridden.HasLongName;
			overridesettingsapplied = true;

			if(!overridden.IsImageLoaded) overridden.LoadImage();
			if(overridden.ImageState == ImageLoadState.Ready)
			{
				// Store source properteis
				sourcesize = new Size(overridden.Width, overridden.Height);
				sourcescale = overridden.Scale;
			}
		}

		// This loads the image
		protected override void LocalLoadImage()
		{
			// Checks
			if(this.IsImageLoaded) return;

			lock(this)
			{ 
				// Get the patch data stream
				if(bitmap != null) bitmap.Dispose(); bitmap = null;
				string sourcelocation = string.Empty;
				Stream data = General.Map.Data.GetHiResTextureData(shortname, ref sourcelocation);
				if(data != null)
				{
					// Copy patch data to memory
					byte[] membytes = new byte[(int)data.Length];

					lock(data) //mxd
					{
						data.Seek(0, SeekOrigin.Begin);
						data.Read(membytes, 0, (int)data.Length);
					}
					
					MemoryStream mem = new MemoryStream(membytes);
					mem.Seek(0, SeekOrigin.Begin);

					// Get a reader for the data
					IImageReader reader = ImageDataFormat.GetImageReader(mem, (isFlat ? ImageDataFormat.DOOMFLAT : ImageDataFormat.DOOMPICTURE), General.Map.Data.Palette);
					if(!(reader is UnknownImageReader))
					{
						// Load the image
						mem.Seek(0, SeekOrigin.Begin);
						try { bitmap = reader.ReadAsBitmap(mem); }
						catch(InvalidDataException)
						{
							// Data cannot be read!
							bitmap = null;
						}
					}

					// Not loaded?
					if(bitmap == null)
					{
						General.ErrorLogger.Add(ErrorType.Error, "Image lump \"" + Path.Combine(sourcelocation, filepathname.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)) + "\" data format could not be read, while loading HiRes texture \"" + this.Name + "\". Does this lump contain valid picture data at all?");
						loadfailed = true;
					}
					else
					{
						// Get width and height from image
						width = bitmap.Size.Width;
						height = bitmap.Size.Height;

						// Apply source overrides?
						if(!sourcesize.IsEmpty)
						{
							scale = new Vector2D(ScaledWidth / width, ScaledHeight / height);
						}
						else 
						{
							if(overridesettingsapplied)
								General.ErrorLogger.Add(ErrorType.Warning, "Unable to get source texture dimensions while loading HiRes texture \"" + this.Name + "\".");

							// Use our own size...
							sourcesize = new Size(width, height);
						}
					}

					// Done
					mem.Dispose();
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Error, "Image lump \"" + shortname + "\" could not be found, while loading HiRes texture \"" + this.Name + "\". Did you forget to include required resources?");
					loadfailed = true;
				}
				
				// Pass on to base
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
