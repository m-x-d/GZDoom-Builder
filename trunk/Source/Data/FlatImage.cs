using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.IO;

namespace CodeImp.DoomBuilder.Data
{
	public sealed class FlatImage : ImageData
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FlatImage(string name)
		{
			// Initialize
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This loads the image
		public override void LoadImage()
		{
			Stream lumpdata;
			MemoryStream mem;
			IImageReader reader;
			byte[] membytes;
			
			// Leave when already loaded
			if(this.IsLoaded) return;

			// Get the lump data stream
			lumpdata = General.Map.Data.GetPatchData(Name);
			if(lumpdata != null)
			{
				// Copy lump data to memory
				lumpdata.Seek(0, SeekOrigin.Begin);
				membytes = new byte[(int)lumpdata.Length];
				lumpdata.Read(membytes, 0, (int)lumpdata.Length);
				mem = new MemoryStream(membytes);
				mem.Seek(0, SeekOrigin.Begin);

				// Get a reader for the data
				reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
				if(reader is UnknownImageReader)
				{
					// Data is in an unknown format!
					General.WriteLogLine("WARNING: Flat lump '" + Name + "' data format could not be read!");
				}

				// Read data as bitmap
				mem.Seek(0, SeekOrigin.Begin);
				bitmap = reader.ReadAsBitmap(mem);
			}
			else
			{
				// Missing a patch lump!
				General.WriteLogLine("WARNING: Missing flat lump '" + Name + "'!");
			}
			
			// Pass on to base
			base.LoadImage();
		}

		#endregion
	}
}
